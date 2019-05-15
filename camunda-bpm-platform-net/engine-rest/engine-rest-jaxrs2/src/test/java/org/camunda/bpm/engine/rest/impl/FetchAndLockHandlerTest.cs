using System;
using System.Collections.Generic;

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
	using ExternalTaskQueryTopicBuilder = org.camunda.bpm.engine.externaltask.ExternalTaskQueryTopicBuilder;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using FetchExternalTasksExtendedDto = org.camunda.bpm.engine.rest.dto.externaltask.FetchExternalTasksExtendedDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using IsCollectionWithSize = org.hamcrest.collection.IsCollectionWithSize;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using ArgumentCaptor = org.mockito.ArgumentCaptor;
	using Mock = org.mockito.Mock;
	using Spy = org.mockito.Spy;
	using MockitoJUnitRunner = org.mockito.runners.MockitoJUnitRunner;



//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyInt;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyListOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyLong;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.argThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doNothing;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doReturn;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doThrow;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.never;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.times;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(MockitoJUnitRunner.class) public class FetchAndLockHandlerTest
	public class FetchAndLockHandlerTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Mock protected org.camunda.bpm.engine.ProcessEngine processEngine;
		protected internal ProcessEngine processEngine;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Mock protected org.camunda.bpm.engine.IdentityService identityService;
	  protected internal IdentityService identityService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Mock protected org.camunda.bpm.engine.ExternalTaskService externalTaskService;
	  protected internal ExternalTaskService externalTaskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Mock protected org.camunda.bpm.engine.externaltask.ExternalTaskQueryTopicBuilder fetchTopicBuilder;
	  protected internal ExternalTaskQueryTopicBuilder fetchTopicBuilder;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Spy protected FetchAndLockHandlerImpl handler;
	  protected internal FetchAndLockHandlerImpl handler;

	  protected internal LockedExternalTask lockedExternalTaskMock;

	  protected internal static readonly DateTime START_DATE = new DateTime(1457326800000L);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initMocks()
	  public virtual void initMocks()
	  {
		when(processEngine.IdentityService).thenReturn(identityService);
		when(processEngine.ExternalTaskService).thenReturn(externalTaskService);
		when(processEngine.Name).thenReturn("default");

		when(externalTaskService.fetchAndLock(anyInt(), any(typeof(string)), any(typeof(Boolean)))).thenReturn(fetchTopicBuilder);
		when(fetchTopicBuilder.topic(any(typeof(string)), anyLong())).thenReturn(fetchTopicBuilder);
		when(fetchTopicBuilder.variables(anyListOf(typeof(string)))).thenReturn(fetchTopicBuilder);
		when(fetchTopicBuilder.enableCustomObjectDeserialization()).thenReturn(fetchTopicBuilder);

		doNothing().when(handler).suspend(anyLong());
		doReturn(processEngine).when(handler).getProcessEngine(any(typeof(FetchAndLockRequest)));

		lockedExternalTaskMock = MockProvider.createMockLockedExternalTask();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setClock()
	  public virtual void setClock()
	  {
		ClockUtil.CurrentTime = START_DATE;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetClock()
	  public virtual void resetClock()
	  {
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void resetUniqueWorkerRequestParam()
	  public virtual void resetUniqueWorkerRequestParam()
	  {
		handler.parseUniqueWorkerRequestParam("false");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResumeAsyncResponseDueToAvailableTasks()
	  public virtual void shouldResumeAsyncResponseDueToAvailableTasks()
	  {
		// given
		IList<LockedExternalTask> tasks = new List<LockedExternalTask>();
		tasks.Add(lockedExternalTaskMock);
		doReturn(tasks).when(fetchTopicBuilder).execute();

		AsyncResponse asyncResponse = mock(typeof(AsyncResponse));
		handler.addPendingRequest(createDto(5000L), asyncResponse, processEngine);

		// when
		handler.acquire();

		// then
		verify(asyncResponse).resume(argThat(IsCollectionWithSize.hasSize(1)));
		assertThat(handler.PendingRequests.Count, @is(0));
		verify(handler).suspend(long.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotResumeAsyncResponseDueToNoAvailableTasks()
	  public virtual void shouldNotResumeAsyncResponseDueToNoAvailableTasks()
	  {
		// given
		doReturn(Collections.emptyList()).when(fetchTopicBuilder).execute();

		AsyncResponse asyncResponse = mock(typeof(AsyncResponse));
		handler.addPendingRequest(createDto(5000L), asyncResponse, processEngine);

		// when
		handler.acquire();

		// then
		verify(asyncResponse, never()).resume(any());
		assertThat(handler.PendingRequests.Count, @is(1));
		verify(handler).suspend(5000L);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResumeAsyncResponseDueToTimeoutExpired_1()
	  public virtual void shouldResumeAsyncResponseDueToTimeoutExpired_1()
	  {
		// given
		doReturn(Collections.emptyList()).when(fetchTopicBuilder).execute();

		AsyncResponse asyncResponse = mock(typeof(AsyncResponse));
		handler.addPendingRequest(createDto(5000L), asyncResponse, processEngine);
		handler.acquire();

		// assume
		assertThat(handler.PendingRequests.Count, @is(1));
		verify(handler).suspend(5000L);

		IList<LockedExternalTask> tasks = new List<LockedExternalTask>();
		tasks.Add(lockedExternalTaskMock);
		doReturn(tasks).when(fetchTopicBuilder).execute();

		addSecondsToClock(5);

		// when
		handler.acquire();

		// then
		verify(asyncResponse).resume(argThat(IsCollectionWithSize.hasSize(1)));
		assertThat(handler.PendingRequests.Count, @is(0));
		verify(handler).suspend(long.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResumeAsyncResponseDueToTimeoutExpired_2()
	  public virtual void shouldResumeAsyncResponseDueToTimeoutExpired_2()
	  {
		// given
		doReturn(Collections.emptyList()).when(fetchTopicBuilder).execute();

		AsyncResponse asyncResponse = mock(typeof(AsyncResponse));
		handler.addPendingRequest(createDto(5000L), asyncResponse, processEngine);

		addSecondsToClock(1);
		handler.acquire();

		// assume
		assertThat(handler.PendingRequests.Count, @is(1));
		verify(handler).suspend(4000L);

		addSecondsToClock(4);

		// when
		handler.acquire();

		// then
		verify(asyncResponse).resume(argThat(IsCollectionWithSize.hasSize(0)));
		assertThat(handler.PendingRequests.Count, @is(0));
		verify(handler).suspend(long.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResumeAsyncResponseDueToTimeoutExpired_3()
	  public virtual void shouldResumeAsyncResponseDueToTimeoutExpired_3()
	  {
		// given
		doReturn(Collections.emptyList()).when(fetchTopicBuilder).execute();

		AsyncResponse asyncResponse = mock(typeof(AsyncResponse));
		handler.addPendingRequest(createDto(5000L), asyncResponse, processEngine);
		handler.addPendingRequest(createDto(4000L), asyncResponse, processEngine);

		addSecondsToClock(1);
		handler.acquire();

		// assume
		assertThat(handler.PendingRequests.Count, @is(2));
		verify(handler).suspend(3000L);

		addSecondsToClock(4);

		// when
		handler.acquire();

		// then
		verify(asyncResponse, times(2)).resume(Collections.emptyList());
		assertThat(handler.PendingRequests.Count, @is(0));
		verify(handler).suspend(long.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResumeAsyncResponseImmediatelyDueToProcessEngineException()
	  public virtual void shouldResumeAsyncResponseImmediatelyDueToProcessEngineException()
	  {
		// given
		doThrow(new ProcessEngineException()).when(fetchTopicBuilder).execute();

		// when
		AsyncResponse asyncResponse = mock(typeof(AsyncResponse));
		handler.addPendingRequest(createDto(5000L), asyncResponse, processEngine);

		// Then
		assertThat(handler.PendingRequests.Count, @is(0));
		verify(handler, never()).suspend(anyLong());
		verify(asyncResponse).resume(any(typeof(ProcessEngineException)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResumeAsyncResponseAfterBackoffDueToProcessEngineException()
	  public virtual void shouldResumeAsyncResponseAfterBackoffDueToProcessEngineException()
	  {
		// given
		doReturn(Collections.emptyList()).when(fetchTopicBuilder).execute();

		AsyncResponse asyncResponse = mock(typeof(AsyncResponse));
		handler.addPendingRequest(createDto(5000L), asyncResponse, processEngine);
		handler.acquire();

		// assume
		assertThat(handler.PendingRequests.Count, @is(1));
		verify(handler).suspend(5000L);

		// when
		doThrow(new ProcessEngineException()).when(fetchTopicBuilder).execute();
		handler.acquire();

		// then
		assertThat(handler.PendingRequests.Count, @is(0));
		verify(handler).suspend(long.MaxValue);
		verify(asyncResponse).resume(any(typeof(ProcessEngineException)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResumeAsyncResponseDueToTimeoutExceeded()
	  public virtual void shouldResumeAsyncResponseDueToTimeoutExceeded()
	  {
		// given - no pending requests

		// assume
		assertThat(handler.PendingRequests.Count, @is(0));

		// when
		AsyncResponse asyncResponse = mock(typeof(AsyncResponse));
		handler.addPendingRequest(createDto(FetchAndLockHandlerImpl.MAX_REQUEST_TIMEOUT + 1), asyncResponse, processEngine);

		// then
		verify(handler, never()).suspend(anyLong());
		assertThat(handler.PendingRequests.Count, @is(0));

		ArgumentCaptor<InvalidRequestException> argumentCaptor = ArgumentCaptor.forClass(typeof(InvalidRequestException));
		verify(asyncResponse).resume(argumentCaptor.capture());
		assertThat(argumentCaptor.Value.Message, @is("The asynchronous response timeout cannot " + "be set to a value greater than " + FetchAndLockHandlerImpl.MAX_REQUEST_TIMEOUT + " milliseconds"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldPollPeriodicallyWhenRequestPending()
	  public virtual void shouldPollPeriodicallyWhenRequestPending()
	  {
		// given
		doReturn(Collections.emptyList()).when(fetchTopicBuilder).execute();

		// when
		AsyncResponse asyncResponse = mock(typeof(AsyncResponse));
		handler.addPendingRequest(createDto(FetchAndLockHandlerImpl.MAX_REQUEST_TIMEOUT), asyncResponse, processEngine);
		handler.acquire();

		// then
		verify(handler).suspend(FetchAndLockHandlerImpl.PENDING_REQUEST_FETCH_INTERVAL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotPollPeriodicallyWhenNotRequestsPending()
	  public virtual void shouldNotPollPeriodicallyWhenNotRequestsPending()
	  {
		// given
		doReturn(Collections.emptyList()).when(fetchTopicBuilder).execute();

		// when
		handler.acquire();

		// then
		verify(handler).suspend(FetchAndLockHandlerImpl.MAX_BACK_OFF_TIME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldCancelPreviousPendingRequestWhenWorkerIdsEqual()
	  public virtual void shouldCancelPreviousPendingRequestWhenWorkerIdsEqual()
	  {
		// given
		doReturn(Collections.emptyList()).when(fetchTopicBuilder).execute();

		handler.parseUniqueWorkerRequestParam("true");

		AsyncResponse asyncResponse = mock(typeof(AsyncResponse));
		handler.addPendingRequest(createDto(FetchAndLockHandlerImpl.MAX_REQUEST_TIMEOUT, "aWorkerId"), asyncResponse, processEngine);
		handler.acquire();

		handler.addPendingRequest(createDto(FetchAndLockHandlerImpl.MAX_REQUEST_TIMEOUT, "aWorkerId"), mock(typeof(AsyncResponse)), processEngine);

		// when
		handler.acquire();

		// then
		verify(asyncResponse).cancel();
		assertThat(handler.PendingRequests.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotCancelPreviousPendingRequestWhenWorkerIdsDiffer()
	  public virtual void shouldNotCancelPreviousPendingRequestWhenWorkerIdsDiffer()
	  {
		// given
		doReturn(Collections.emptyList()).when(fetchTopicBuilder).execute();

		handler.parseUniqueWorkerRequestParam("true");

		AsyncResponse asyncResponse = mock(typeof(AsyncResponse));
		handler.addPendingRequest(createDto(FetchAndLockHandlerImpl.MAX_REQUEST_TIMEOUT, "aWorkerId"), asyncResponse, processEngine);
		handler.acquire();

		handler.addPendingRequest(createDto(FetchAndLockHandlerImpl.MAX_REQUEST_TIMEOUT, "anotherWorkerId"), mock(typeof(AsyncResponse)), processEngine);

		// when
		handler.acquire();

		// then
		verify(asyncResponse, never()).cancel();
		assertThat(handler.PendingRequests.Count, @is(2));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResumeAsyncResponseDueToTooManyRequests()
	  public virtual void shouldResumeAsyncResponseDueToTooManyRequests()
	  {
		// given

		// when
		AsyncResponse asyncResponse = mock(typeof(AsyncResponse));
		handler.errorTooManyRequests(asyncResponse);

		// then
		ArgumentCaptor<InvalidRequestException> argumentCaptor = ArgumentCaptor.forClass(typeof(InvalidRequestException));
		verify(asyncResponse).resume(argumentCaptor.capture());
		assertThat(argumentCaptor.Value.Message, @is("At the moment the server has to handle too " + "many requests at the same time. Please try again later."));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldSuspendForeverDueToNoPendingRequests()
	  public virtual void shouldSuspendForeverDueToNoPendingRequests()
	  {
		// given - no pending requests

		// assume
		assertThat(handler.PendingRequests.Count, @is(0));

		// when
		handler.acquire();

		// then
		assertThat(handler.PendingRequests.Count, @is(0));
		verify(handler).suspend(long.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldRejectRequestDueToShutdown()
	  public virtual void shouldRejectRequestDueToShutdown()
	  {
		// given
		AsyncResponse asyncResponse = mock(typeof(AsyncResponse));
		handler.addPendingRequest(createDto(5000L), asyncResponse, processEngine);
		handler.acquire();

		// assume
		assertThat(handler.PendingRequests.Count, @is(1));

		// when
		handler.rejectPendingRequests();

		// then
		ArgumentCaptor<RestException> argumentCaptor = ArgumentCaptor.forClass(typeof(RestException));
		verify(asyncResponse).resume(argumentCaptor.capture());
		assertThat(argumentCaptor.Value.Status, @is(Status.INTERNAL_SERVER_ERROR));
		assertThat(argumentCaptor.Value.Message, @is("Request rejected due to shutdown of application server."));
	  }

	  protected internal virtual FetchExternalTasksExtendedDto createDto(long? responseTimeout, string workerId)
	  {
		FetchExternalTasksExtendedDto externalTask = new FetchExternalTasksExtendedDto();

		FetchExternalTasksExtendedDto.FetchExternalTaskTopicDto topic = new FetchExternalTasksExtendedDto.FetchExternalTaskTopicDto();
		topic.TopicName = "aTopicName";
		topic.LockDuration = 12354L;

		externalTask.MaxTasks = 5;
		externalTask.WorkerId = workerId;
		externalTask.Topics = Collections.singletonList(topic);

		if (responseTimeout != null)
		{
		  externalTask.AsyncResponseTimeout = responseTimeout;
		}

		return externalTask;
	  }

	  protected internal virtual FetchExternalTasksExtendedDto createDto(long? responseTimeout)
	  {
		return createDto(responseTimeout, "aWorkerId");
	  }

	  protected internal virtual DateTime addSeconds(DateTime date, int seconds)
	  {
		return new DateTime(date.Ticks + seconds * 1000);
	  }

	  protected internal virtual void addSecondsToClock(int seconds)
	  {
		DateTime newDate = addSeconds(ClockUtil.CurrentTime, seconds);
		ClockUtil.CurrentTime = newDate;
	  }

	}

}