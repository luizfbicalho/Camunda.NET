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
namespace org.camunda.bpm.container.impl.threading.ra.outbound
{



//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ConnectionDefinition(connectionFactory = JcaExecutorServiceConnectionFactory.class, connectionFactoryImpl = JcaExecutorServiceConnectionFactoryImpl.class, connection = JcaExecutorServiceConnection.class, connectionImpl = JcaExecutorServiceConnectionImpl.class) public class JcaExecutorServiceManagedConnectionFactory implements javax.resource.spi.ManagedConnectionFactory, javax.resource.spi.ResourceAdapterAssociation
	public class JcaExecutorServiceManagedConnectionFactory : ManagedConnectionFactory, ResourceAdapterAssociation
	{

	  private const long serialVersionUID = 1L;

	  protected internal ResourceAdapter ra;
	  protected internal PrintWriter logwriter;

	  public JcaExecutorServiceManagedConnectionFactory()
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object createConnectionFactory(javax.resource.spi.ConnectionManager cxManager) throws javax.resource.ResourceException
	  public virtual object createConnectionFactory(ConnectionManager cxManager)
	  {
		return new JcaExecutorServiceConnectionFactoryImpl(this, cxManager);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object createConnectionFactory() throws javax.resource.ResourceException
	  public virtual object createConnectionFactory()
	  {
		throw new ResourceException("This resource adapter doesn't support non-managed environments");
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.resource.spi.ManagedConnection createManagedConnection(javax.security.auth.Subject subject, javax.resource.spi.ConnectionRequestInfo cxRequestInfo) throws javax.resource.ResourceException
	  public virtual ManagedConnection createManagedConnection(Subject subject, ConnectionRequestInfo cxRequestInfo)
	  {
		return new JcaExecutorServiceManagedConnection(this);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public javax.resource.spi.ManagedConnection matchManagedConnections(java.util.Set connectionSet, javax.security.auth.Subject subject, javax.resource.spi.ConnectionRequestInfo cxRequestInfo) throws javax.resource.ResourceException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual ManagedConnection matchManagedConnections(ISet<object> connectionSet, Subject subject, ConnectionRequestInfo cxRequestInfo)
	  {
		ManagedConnection result = null;
		System.Collections.IEnumerator it = connectionSet.GetEnumerator();
		while (result == null && it.MoveNext())
		{
		  ManagedConnection mc = (ManagedConnection) it.Current;
		  if (mc is JcaExecutorServiceManagedConnection)
		  {
			result = mc;
		  }

		}
		return result;
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


	  public virtual ResourceAdapter ResourceAdapter
	  {
		  get
		  {
			return ra;
		  }
		  set
		  {
			this.ra = value;
		  }
	  }


	  public override int GetHashCode()
	  {
		return 31;
	  }

	  public override bool Equals(object other)
	  {
		if (other == null)
		{
		  return false;
		}
		if (other == this)
		{
		  return true;
		}
		if (!(other is JcaExecutorServiceManagedConnectionFactory))
		{
		  return false;
		}
		return true;
	  }

	}

}