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
	using OperationContext = org.jboss.@as.controller.OperationContext;
	using OperationFailedException = org.jboss.@as.controller.OperationFailedException;
	using ReloadRequiredRemoveStepHandler = org.jboss.@as.controller.ReloadRequiredRemoveStepHandler;
	using ModelNode = org.jboss.dmr.ModelNode;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Christian Lipphardt
	/// </summary>
	public class BpmPlatformSubsystemRemove : ReloadRequiredRemoveStepHandler
	{

	  public static readonly BpmPlatformSubsystemRemove INSTANCE = new BpmPlatformSubsystemRemove();

	  private BpmPlatformSubsystemRemove()
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void performRuntime(org.jboss.as.controller.OperationContext context, org.jboss.dmr.ModelNode operation, org.jboss.dmr.ModelNode model) throws org.jboss.as.controller.OperationFailedException
	  protected internal virtual void performRuntime(OperationContext context, ModelNode operation, ModelNode model)
	  {
		base.performRuntime(context, operation, model);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void recoverServices(org.jboss.as.controller.OperationContext context, org.jboss.dmr.ModelNode operation, org.jboss.dmr.ModelNode model) throws org.jboss.as.controller.OperationFailedException
	  protected internal virtual void recoverServices(OperationContext context, ModelNode operation, ModelNode model)
	  {
		base.recoverServices(context, operation, model);

	  }
	}

}