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
namespace org.camunda.connect.plugin.impl
{

	using TaskActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.TaskActivityBehavior;
	using IoMapping = org.camunda.bpm.engine.impl.core.variable.mapping.IoMapping;
	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using Connector = org.camunda.connect.spi.Connector;
	using ConnectorRequest = org.camunda.connect.spi.ConnectorRequest;
	using ConnectorResponse = org.camunda.connect.spi.ConnectorResponse;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ServiceTaskConnectorActivityBehavior : TaskActivityBehavior
	{

	  /// <summary>
	  /// the id of the connector </summary>
	  protected internal string connectorId;

	  /// <summary>
	  /// cached connector instance for this activity.
	  /// Will be initialized after the first execution of this activity. 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected org.camunda.connect.spi.Connector<?> connectorInstance;
	  protected internal Connector<object> connectorInstance;

	  /// <summary>
	  /// the local ioMapping for this connector. </summary>
	  protected internal IoMapping ioMapping;

	  public ServiceTaskConnectorActivityBehavior(string connectorId, IoMapping ioMapping)
	  {
		this.connectorId = connectorId;
		this.ioMapping = ioMapping;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(final org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void execute(ActivityExecution execution)
	  {
		ensureConnectorInitialized();

		executeWithErrorPropagation(execution, new CallableAnonymousInnerClass(this, execution));

	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly ServiceTaskConnectorActivityBehavior outerInstance;

		  private ActivityExecution execution;

		  public CallableAnonymousInnerClass(ServiceTaskConnectorActivityBehavior outerInstance, ActivityExecution execution)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Void call() throws Exception
		  public override Void call()
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.connect.spi.ConnectorRequest<?> request = connectorInstance.createRequest();
			ConnectorRequest<object> request = outerInstance.connectorInstance.createRequest();
			outerInstance.applyInputParameters(execution, request);
			// execute the request and obtain a response:
			ConnectorResponse response = request.execute();
			outerInstance.applyOutputParameters(execution, response);
			outerInstance.leave(execution);
			return null;
		  }
	  }

	  protected internal virtual void applyInputParameters<T1>(ActivityExecution execution, ConnectorRequest<T1> request)
	  {
		if (ioMapping != null)
		{
		  // create variable scope for input parameters
		  ConnectorVariableScope connectorInputVariableScope = new ConnectorVariableScope((AbstractVariableScope) execution);
		  // execute the connector input parameters
		  ioMapping.executeInputParameters(connectorInputVariableScope);
		  // write the local variables to the request.
		  connectorInputVariableScope.writeToRequest(request);
		}
	  }

	  protected internal virtual void applyOutputParameters(ActivityExecution execution, ConnectorResponse response)
	  {
		if (ioMapping != null)
		{
		  // create variable scope for output parameters
		  ConnectorVariableScope connectorOutputVariableScope = new ConnectorVariableScope((AbstractVariableScope) execution);
		  // read parameters from response
		  connectorOutputVariableScope.readFromResponse(response);
		  // map variables to parent scope.
		  ioMapping.executeOutputParameters(connectorOutputVariableScope);
		}
	  }

	  protected internal virtual void ensureConnectorInitialized()
	  {
		if (connectorInstance == null)
		{
		  lock (this)
		  {
			if (connectorInstance == null)
			{
			  connectorInstance = Connectors.getConnector(connectorId);
			  if (connectorInstance == null)
			  {
				throw new ConnectorException("No connector found for connector id '" + connectorId + "'");
			  }
			}
		  }
		}
	  }

	}

}