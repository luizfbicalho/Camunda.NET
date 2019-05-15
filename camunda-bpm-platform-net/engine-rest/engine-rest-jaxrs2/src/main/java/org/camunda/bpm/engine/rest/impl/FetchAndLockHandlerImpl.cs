using System;
using System.Collections.Generic;
using System.Threading;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.rest.impl
{


	using ExternalTaskQueryBuilder = org.camunda.bpm.engine.externaltask.ExternalTaskQueryBuilder;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using SingleConsumerCondition = org.camunda.bpm.engine.impl.util.SingleConsumerCondition;
	using FetchExternalTasksExtendedDto = org.camunda.bpm.engine.rest.dto.externaltask.FetchExternalTasksExtendedDto;
	using LockedExternalTaskDto = org.camunda.bpm.engine.rest.dto.externaltask.LockedExternalTaskDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using FetchAndLockHandler = org.camunda.bpm.engine.rest.spi.FetchAndLockHandler;
	using EngineUtil = org.camunda.bpm.engine.rest.util.EngineUtil;


	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class FetchAndLockHandlerImpl : ThreadStart, FetchAndLockHandler
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			handlerThread = new Thread(this, this.GetType().Name);
		}


//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger LOG = Logger.getLogger(typeof(FetchAndLockHandlerImpl).FullName);

	  protected internal const string UNIQUE_WORKER_REQUEST_PARAM_NAME = "fetch-and-lock-unique-worker-request";

	  protected internal static readonly long PENDING_REQUEST_FETCH_INTERVAL = 30L * 1000;
	  protected internal static readonly long MAX_BACK_OFF_TIME = long.MaxValue;
	  protected internal const long MAX_REQUEST_TIMEOUT = 1800000; // 30 minutes

	  protected internal SingleConsumerCondition condition;

	  protected internal BlockingQueue<FetchAndLockRequest> queue = new ArrayBlockingQueue<FetchAndLockRequest>(200);
	  protected internal IList<FetchAndLockRequest> pendingRequests = new List<FetchAndLockRequest>();
	  protected internal IList<FetchAndLockRequest> newRequests = new List<FetchAndLockRequest>();

	  protected internal Thread handlerThread;

	  protected internal volatile bool isRunning = false;

	  protected internal bool isUniqueWorkerRequest = false;

	  public FetchAndLockHandlerImpl()
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		this.condition = new SingleConsumerCondition(handlerThread);
	  }

	  public override void run()
	  {
		while (isRunning)
		{
		  try
		  {
			acquire();
		  }
		  catch (Exception)
		  {
			// what ever happens, don't leave the loop
		  }
		}

		rejectPendingRequests();
	  }

	  protected internal virtual void acquire()
	  {
		LOG.log(Level.FINEST, "Acquire start");

		queue.drainTo(newRequests);

		if (newRequests.Count > 0)
		{
		  if (isUniqueWorkerRequest)
		  {
			removeDuplicates();
		  }

		  ((IList<FetchAndLockRequest>)pendingRequests).AddRange(newRequests);
		  newRequests.Clear();
		}

		LOG.log(Level.FINEST, "Number of pending requests {0}", pendingRequests.Count);

		long backoffTime = MAX_BACK_OFF_TIME; //timestamp

		IEnumerator<FetchAndLockRequest> iterator = pendingRequests.GetEnumerator();
		while (iterator.MoveNext())
		{

		  FetchAndLockRequest pendingRequest = iterator.Current;

		  LOG.log(Level.FINEST, "Fetching tasks for request {0}", pendingRequest);

		  FetchAndLockResult result = tryFetchAndLock(pendingRequest);

		  LOG.log(Level.FINEST, "Fetch and lock result: {0}", result);

		  if (result.wasSuccessful())
		  {

			IList<LockedExternalTaskDto> lockedTasks = result.Tasks;

			if (lockedTasks.Count > 0 || isExpired(pendingRequest))
			{
			  AsyncResponse asyncResponse = pendingRequest.AsyncResponse;
			  asyncResponse.resume(lockedTasks);

			  LOG.log(Level.FINEST, "resume and remove request with {0}", lockedTasks);

//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
			  iterator.remove();
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long msUntilTimeout = pendingRequest.getTimeoutTimestamp() - org.camunda.bpm.engine.impl.util.ClockUtil.getCurrentTime().getTime();
			  long msUntilTimeout = pendingRequest.TimeoutTimestamp - ClockUtil.CurrentTime.Ticks;
			  backoffTime = Math.Min(backoffTime, msUntilTimeout);
			}
		  }
		  else
		  {
			AsyncResponse asyncResponse = pendingRequest.AsyncResponse;
			Exception processEngineException = result.Throwable;
			asyncResponse.resume(processEngineException);

			LOG.log(Level.FINEST, "Resume and remove request with error {0}", processEngineException);

//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
			iterator.remove();
		  }
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long waitTime = Math.max(0, backoffTime);
		long waitTime = Math.Max(0, backoffTime);

		if (pendingRequests.Count == 0)
		{
		  suspend(waitTime);
		}
		else
		{
		  // if there are pending requests, try fetch periodically to ensure tasks created on other
		  // cluster nodes and tasks with expired timeouts can be fetched in a timely manner
		  suspend(Math.Min(PENDING_REQUEST_FETCH_INTERVAL, waitTime));
		}
	  }

	  protected internal virtual void removeDuplicates()
	  {
		foreach (FetchAndLockRequest newRequest in newRequests)
		{
		  // remove any request from pendingRequests with the same worker id
		  IEnumerator<FetchAndLockRequest> iterator = pendingRequests.GetEnumerator();
		  while (iterator.MoveNext())
		  {
			FetchAndLockRequest pendingRequest = iterator.Current;
			if (pendingRequest.Dto.WorkerId.Equals(newRequest.Dto.WorkerId))
			{
			  AsyncResponse asyncResponse = pendingRequest.AsyncResponse;
			  asyncResponse.cancel();

//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
			  iterator.remove();
			}
		  }

		}
	  }

	  public virtual void start()
	  {
		if (isRunning)
		{
		  return;
		}

		isRunning = true;
		handlerThread.Start();

		ProcessEngineImpl.EXT_TASK_CONDITIONS.addConsumer(condition);
	  }

	  public virtual void shutdown()
	  {
		try
		{
		  ProcessEngineImpl.EXT_TASK_CONDITIONS.removeConsumer(condition);
		}
		finally
		{
		  isRunning = false;
		  condition.signal();
		}

		try
		{
		  handlerThread.Join();
		}
		catch (InterruptedException e)
		{
		  LOG.log(Level.WARNING, "Shutting down the handler thread failed: {0}", e);
		}
	  }

	  protected internal virtual void suspend(long millis)
	  {
		if (millis <= 0)
		{
		  return;
		}

		suspendAcquisition(millis);
	  }

	  protected internal virtual void suspendAcquisition(long millis)
	  {
		try
		{
		  if (queue.Empty && isRunning)
		  {
			LOG.log(Level.FINEST, "Suspend acquisition for {0}ms", millis);
			condition.await(millis);
			LOG.log(Level.FINEST, "Acquisition woke up");
		  }
		}
		finally
		{
		  if (handlerThread.Interrupted)
		  {
			Thread.CurrentThread.Interrupt();
		  }
		}
	  }

	  protected internal virtual void addRequest(FetchAndLockRequest request)
	  {
		if (!queue.offer(request))
		{
		  AsyncResponse asyncResponse = request.AsyncResponse;
		  errorTooManyRequests(asyncResponse);
		}

		condition.signal();
	  }

	  protected internal virtual FetchAndLockResult tryFetchAndLock(FetchAndLockRequest request)
	  {

		ProcessEngine processEngine = null;
		IdentityService identityService = null;
		FetchAndLockResult result = null;

		try
		{
		  processEngine = getProcessEngine(request);

		  identityService = processEngine.IdentityService;
		  identityService.Authentication = request.Authentication;

		  FetchExternalTasksExtendedDto fetchingDto = request.Dto;
		  IList<LockedExternalTaskDto> lockedTasks = executeFetchAndLock(fetchingDto, processEngine);
		  result = FetchAndLockResult.successful(lockedTasks);
		}
		catch (Exception e)
		{
		  result = FetchAndLockResult.failed(e);
		}
		finally
		{
		  if (identityService != null)
		  {
			identityService.clearAuthentication();
		  }
		}

		return result;
	  }

	  protected internal virtual IList<LockedExternalTaskDto> executeFetchAndLock(FetchExternalTasksExtendedDto fetchingDto, ProcessEngine processEngine)
	  {
		ExternalTaskQueryBuilder fetchBuilder = fetchingDto.buildQuery(processEngine);
		IList<LockedExternalTask> externalTasks = fetchBuilder.execute();
		return LockedExternalTaskDto.fromLockedExternalTasks(externalTasks);
	  }

	  protected internal virtual void errorTooManyRequests(AsyncResponse asyncResponse)
	  {
		string errorMessage = "At the moment the server has to handle too many requests at the same time. Please try again later.";
		asyncResponse.resume(new InvalidRequestException(Status.INTERNAL_SERVER_ERROR, errorMessage));
	  }

	  protected internal virtual void rejectPendingRequests()
	  {
		foreach (FetchAndLockRequest pendingRequest in pendingRequests)
		{
		  AsyncResponse asyncResponse = pendingRequest.AsyncResponse;
		  asyncResponse.resume(new RestException(Status.INTERNAL_SERVER_ERROR, "Request rejected due to shutdown of application server."));
		}
	  }

	  protected internal virtual ProcessEngine getProcessEngine(FetchAndLockRequest request)
	  {
		string processEngineName = request.ProcessEngineName;
		return EngineUtil.lookupProcessEngine(processEngineName);
	  }

	  protected internal virtual bool isExpired(FetchAndLockRequest request)
	  {
		long currentTime = ClockUtil.CurrentTime.Ticks;
		long timeout = request.TimeoutTimestamp;
		return timeout <= currentTime;
	  }

	  public virtual void addPendingRequest(FetchExternalTasksExtendedDto dto, AsyncResponse asyncResponse, ProcessEngine processEngine)
	  {
		long? asyncResponseTimeout = dto.AsyncResponseTimeout;
		if (asyncResponseTimeout != null && asyncResponseTimeout.Value > MAX_REQUEST_TIMEOUT)
		{
		  asyncResponse.resume(new InvalidRequestException(Status.BAD_REQUEST, "The asynchronous response timeout cannot be set to a value greater than " + MAX_REQUEST_TIMEOUT + " milliseconds"));
		  return;
		}

		IdentityService identityService = processEngine.IdentityService;
		Authentication authentication = identityService.CurrentAuthentication;
		string processEngineName = processEngine.Name;

		FetchAndLockRequest incomingRequest = (new FetchAndLockRequest()).setProcessEngineName(processEngineName).setAsyncResponse(asyncResponse).setAuthentication(authentication).setDto(dto);

		LOG.log(Level.FINEST, "New request: {0}", incomingRequest);

		FetchAndLockResult result = tryFetchAndLock(incomingRequest);

		LOG.log(Level.FINEST, "Fetch and lock result: {0}", result);

		if (result.wasSuccessful())
		{
		  IList<LockedExternalTaskDto> lockedTasks = result.Tasks;
		  if (lockedTasks.Count > 0 || dto.AsyncResponseTimeout == null)
		  { // response immediately if tasks available
			asyncResponse.resume(lockedTasks);

			LOG.log(Level.FINEST, "Resuming request with {0}", lockedTasks);
		  }
		  else
		  {
			addRequest(incomingRequest);

			LOG.log(Level.FINEST, "Deferred request");
		  }
		}
		else
		{
		  Exception processEngineException = result.Throwable;
		  asyncResponse.resume(processEngineException);

		  LOG.log(Level.FINEST, "Resuming request with error {0}", processEngineException);
		}
	  }

	  public virtual void contextInitialized(ServletContextEvent servletContextEvent)
	  {
		ServletContext servletContext = null;

		if (servletContextEvent != null)
		{
		  servletContext = servletContextEvent.ServletContext;

		  if (servletContext != null)
		  {
			parseUniqueWorkerRequestParam(servletContext.getInitParameter(UNIQUE_WORKER_REQUEST_PARAM_NAME));
		  }
		}
	  }

	  protected internal virtual void parseUniqueWorkerRequestParam(string uniqueWorkerRequestParam)
	  {
		if (!string.ReferenceEquals(uniqueWorkerRequestParam, null))
		{
		  isUniqueWorkerRequest = Convert.ToBoolean(uniqueWorkerRequestParam);
		}
		else
		{
		  isUniqueWorkerRequest = false; // default configuration
		}
	  }

	  public virtual IList<FetchAndLockRequest> PendingRequests
	  {
		  get
		  {
			return pendingRequests;
		  }
	  }
	}

}