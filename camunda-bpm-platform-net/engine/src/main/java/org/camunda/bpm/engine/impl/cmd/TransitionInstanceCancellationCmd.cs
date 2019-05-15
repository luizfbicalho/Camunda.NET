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
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class TransitionInstanceCancellationCmd : AbstractInstanceCancellationCmd
	{

	  protected internal string transitionInstanceId;

	  public TransitionInstanceCancellationCmd(string processInstanceId, string transitionInstanceId) : base(processInstanceId)
	  {
		this.transitionInstanceId = transitionInstanceId;

	  }

	  public virtual string TransitionInstanceId
	  {
		  get
		  {
			return transitionInstanceId;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity determineSourceInstanceExecution(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  protected internal override ExecutionEntity determineSourceInstanceExecution(CommandContext commandContext)
	  {
		ActivityInstance instance = commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext));
		TransitionInstance instanceToCancel = findTransitionInstance(instance, transitionInstanceId);
		EnsureUtil.ensureNotNull(typeof(NotValidException), describeFailure("Transition instance '" + transitionInstanceId + "' does not exist"), "transitionInstance", instanceToCancel);

		ExecutionEntity transitionExecution = commandContext.ExecutionManager.findExecutionById(instanceToCancel.ExecutionId);

		return transitionExecution;
	  }

	  private class CallableAnonymousInnerClass : Callable<ActivityInstance>
	  {
		  private readonly TransitionInstanceCancellationCmd outerInstance;

		  private CommandContext commandContext;

		  public CallableAnonymousInnerClass(TransitionInstanceCancellationCmd outerInstance, CommandContext commandContext)
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
		return "Cancel transition instance '" + transitionInstanceId + "'";
	  }


	}

}