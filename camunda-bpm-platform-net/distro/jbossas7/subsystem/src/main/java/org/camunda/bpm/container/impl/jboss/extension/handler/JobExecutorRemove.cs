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
namespace org.camunda.bpm.container.impl.jboss.extension.handler
{
	using ServiceNames = org.camunda.bpm.container.impl.jboss.service.ServiceNames;
	using AbstractRemoveStepHandler = org.jboss.@as.controller.AbstractRemoveStepHandler;
	using OperationContext = org.jboss.@as.controller.OperationContext;
	using OperationFailedException = org.jboss.@as.controller.OperationFailedException;
	using ModelNode = org.jboss.dmr.ModelNode;
	using ServiceName = org.jboss.msc.service.ServiceName;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Christian Lipphardt
	/// 
	/// </summary>
	public class JobExecutorRemove : AbstractRemoveStepHandler
	{

	  public static JobExecutorRemove INSTANCE = new JobExecutorRemove();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void performRuntime(org.jboss.as.controller.OperationContext context, org.jboss.dmr.ModelNode operation, org.jboss.dmr.ModelNode model) throws org.jboss.as.controller.OperationFailedException
	  protected internal virtual void performRuntime(OperationContext context, ModelNode operation, ModelNode model)
	  {
		ServiceName name = ServiceNames.forMscExecutorService();
		context.removeService(name);
	  }

	}

}