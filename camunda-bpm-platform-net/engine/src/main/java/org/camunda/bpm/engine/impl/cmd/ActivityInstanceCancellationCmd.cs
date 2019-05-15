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
namespace org.camunda.bpm.engine.impl.cmd
{

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ActivityInstanceCancellationCmd : AbstractInstanceCancellationCmd
	{

	  protected internal string activityInstanceId;

	  public ActivityInstanceCancellationCmd(string processInstanceId, string activityInstanceId) : base(processInstanceId)
	  {
		this.activityInstanceId = activityInstanceId;
	  }

	  public ActivityInstanceCancellationCmd(string processInstanceId, string activityInstanceId, string cancellationReason) : base(processInstanceId, cancellationReason)
	  {
		this.activityInstanceId = activityInstanceId;
	  }

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity determineSourceInstanceExecution(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  protected internal override ExecutionEntity determineSourceInstanceExecution(CommandContext commandContext)
	  {
		ExecutionEntity processInstance = commandContext.ExecutionManager.findExecutionById(processInstanceId);

		// rebuild the mapping because the execution tree changes with every iteration
		ActivityExecutionTreeMapping mapping = new ActivityExecutionTreeMapping(commandContext, processInstanceId);

		ActivityInstance instance = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext));

		ActivityInstance instanceToCancel = findActivityInstance(instance, activityInstanceId);
		EnsureUtil.ensureNotNull(typeof(NotValidException), describeFailure("Activity instance '" + activityInstanceId + "' does not exist"), "activityInstance", instanceToCancel);
		ExecutionEntity scopeExecution = getScopeExecutionForActivityInstance(processInstance, mapping, instanceToCancel);

		return scopeExecution;
	  }

	  private class CallableAnonymousInnerClass : Callable<ActivityInstance>
	  {
		  private readonly ActivityInstanceCancellationCmd outerInstance;

		  private CommandContext commandContext;

		  public CallableAnonymousInnerClass(ActivityInstanceCancellationCmd outerInstance, CommandContext commandContext)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.runtime.ActivityInstance call() throws Exception
		  public ActivityInstance call()
		  {
			return (new GetActivityInstanceCmd(outerInstance.processInstanceId)).execute(commandContext);
		  }
	  }

	  protected internal override string describe()
	  {
		return "Cancel activity instance '" + activityInstanceId + "'";
	  }


	}

}