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
namespace org.camunda.bpm.container.impl.threading.ra.outbound
{


	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;


	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class JcaExecutorServiceManagedConnection : ManagedConnection
	{

	  protected internal PrintWriter logwriter;

	  protected internal JcaExecutorServiceManagedConnectionFactory mcf;
	  protected internal IList<ConnectionEventListener> listeners;
	  protected internal JcaExecutorServiceConnectionImpl connection;

	  protected internal ExecutorService @delegate;

	  public JcaExecutorServiceManagedConnection()
	  {
	  }

	  public JcaExecutorServiceManagedConnection(JcaExecutorServiceManagedConnectionFactory mcf)
	  {
		this.mcf = mcf;
		this.logwriter = null;
		this.listeners = Collections.synchronizedList(new List<ConnectionEventListener>(1));
		this.connection = null;
		JcaExecutorServiceConnector ra = (JcaExecutorServiceConnector) mcf.ResourceAdapter;
		@delegate = (ExecutorService) ra.getExecutorServiceWrapper().ExecutorService;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getConnection(javax.security.auth.Subject subject, javax.resource.spi.ConnectionRequestInfo cxRequestInfo) throws javax.resource.ResourceException
	  public virtual object getConnection(Subject subject, ConnectionRequestInfo cxRequestInfo)
	  {
		connection = new JcaExecutorServiceConnectionImpl(this, mcf);
		return connection;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void associateConnection(Object connection) throws javax.resource.ResourceException
	  public virtual void associateConnection(object connection)
	  {
		if (connection == null)
		{
		  throw new ResourceException("Null connection handle");
		}
		if (!(connection is JcaExecutorServiceConnectionImpl))
		{
		  throw new ResourceException("Wrong connection handle");
		}
		this.connection = (JcaExecutorServiceConnectionImpl) connection;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void cleanup() throws javax.resource.ResourceException
	  public virtual void cleanup()
	  {
		// no-op
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void destroy() throws javax.resource.ResourceException
	  public virtual void destroy()
	  {
		// no-op
	  }

	  public virtual void addConnectionEventListener(ConnectionEventListener listener)
	  {
		if (listener == null)
		{
		  throw new System.ArgumentException("Listener is null");
		}
		listeners.Add(listener);
	  }

	  public virtual void removeConnectionEventListener(ConnectionEventListener listener)
	  {
		if (listener == null)
		{
		  throw new System.ArgumentException("Listener is null");
		}
		listeners.Remove(listener);
	  }

	  internal virtual void closeHandle(JcaExecutorServiceConnection handle)
	  {
		ConnectionEvent @event = new ConnectionEvent(this, ConnectionEvent.CONNECTION_CLOSED);
		@event.ConnectionHandle = handle;
		foreach (ConnectionEventListener cel in listeners)
		{
		  cel.connectionClosed(@event);
		}

	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.PrintWriter getLogWriter() throws javax.resource.ResourceException
	  public virtual PrintWriter LogWriter
	  {
		  get
		  {
			return logwriter;
		  }
		  set
		  {
			logwriter = value;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.resource.spi.LocalTransaction getLocalTransaction() throws javax.resource.ResourceException
	  public virtual LocalTransaction LocalTransaction
	  {
		  get
		  {
			throw new NotSupportedException("LocalTransaction not supported");
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.transaction.xa.XAResource getXAResource() throws javax.resource.ResourceException
	  public virtual XAResource XAResource
	  {
		  get
		  {
			throw new NotSupportedException("GetXAResource not supported not supported");
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.resource.spi.ManagedConnectionMetaData getMetaData() throws javax.resource.ResourceException
	  public virtual ManagedConnectionMetaData MetaData
	  {
		  get
		  {
			return null;
		  }
	  }

	  // delegate methods /////////////////////////////////////////

	  public virtual bool schedule(ThreadStart runnable, bool isLongRunning)
	  {
		return @delegate.schedule(runnable, isLongRunning);
	  }

	  public virtual ThreadStart getExecuteJobsRunnable(IList<string> jobIds, ProcessEngineImpl processEngine)
	  {
		return @delegate.getExecuteJobsRunnable(jobIds, processEngine);
	  }

	}

}