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
namespace org.camunda.bpm.engine.impl.test
{
	using CaseSentryPartQueryImpl = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseSentryPartQueryImpl;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// <summary>
	/// Base class for CMMN test cases with helper methods.
	/// 
	/// These also includes state transition methods which are currently
	/// not implemented as parted of the public API, i.e. <seealso cref="CaseService"/>.
	/// These methods should be removed after they are available through public API.
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CmmnProcessEngineTestCase : PluggableProcessEngineTestCase
	{

	  // create case instance
	  protected internal virtual CaseInstance createCaseInstance()
	  {
		return createCaseInstance(null);
	  }

	  protected internal virtual CaseInstance createCaseInstance(string businessKey)
	  {
		string caseDefinitionKey = repositoryService.createCaseDefinitionQuery().singleResult().Key;

		return createCaseInstanceByKey(caseDefinitionKey, businessKey);
	  }

	  protected internal virtual CaseInstance createCaseInstanceByKey(string caseDefinitionKey)
	  {
		return createCaseInstanceByKey(caseDefinitionKey, null, null);
	  }

	  protected internal virtual CaseInstance createCaseInstanceByKey(string caseDefinitionKey, string businessKey)
	  {
		return createCaseInstanceByKey(caseDefinitionKey, businessKey, null);
	  }

	  protected internal virtual CaseInstance createCaseInstanceByKey(string caseDefinitionKey, VariableMap variables)
	  {
		return createCaseInstanceByKey(caseDefinitionKey, null, variables);
	  }

	  protected internal virtual CaseInstance createCaseInstanceByKey(string caseDefinitionKey, string businessKey, VariableMap variables)
	  {
		return caseService.withCaseDefinitionByKey(caseDefinitionKey).businessKey(businessKey).setVariables(variables).create();
	  }

	  // queries

	  protected internal virtual CaseExecution queryCaseExecutionByActivityId(string activityId)
	  {
		return caseService.createCaseExecutionQuery().activityId(activityId).singleResult();
	  }

	  protected internal virtual CaseExecution queryCaseExecutionById(string caseExecutionId)
	  {
		return caseService.createCaseExecutionQuery().caseExecutionId(caseExecutionId).singleResult();
	  }

	  protected internal virtual CaseSentryPartQueryImpl createCaseSentryPartQuery()
	  {
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequiresNew;
		return new CaseSentryPartQueryImpl(commandExecutor);
	  }

	  // transition methods

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void close(final String caseExecutionId)
	  protected internal virtual void close(string caseExecutionId)
	  {
		caseService.withCaseExecution(caseExecutionId).close();
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void complete(final String caseExecutionId)
	  protected internal virtual void complete(string caseExecutionId)
	  {
		caseService.withCaseExecution(caseExecutionId).complete();
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.runtime.CaseInstance create(final String caseDefinitionId)
	  protected internal virtual CaseInstance create(string caseDefinitionId)
	  {
		return caseService.withCaseDefinition(caseDefinitionId).create();
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.runtime.CaseInstance create(final String caseDefinitionId, final String businessKey)
	  protected internal virtual CaseInstance create(string caseDefinitionId, string businessKey)
	  {
		return caseService.withCaseDefinition(caseDefinitionId).businessKey(businessKey).create();
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void disable(final String caseExecutionId)
	  protected internal virtual void disable(string caseExecutionId)
	  {
		caseService.withCaseExecution(caseExecutionId).disable();
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void exit(final String caseExecutionId)
	  protected internal virtual void exit(string caseExecutionId)
	  {
		executeHelperCaseCommand(new HelperCaseCommandAnonymousInnerClass(this, caseExecutionId));
	  }

	  private class HelperCaseCommandAnonymousInnerClass : HelperCaseCommand
	  {
		  private readonly CmmnProcessEngineTestCase outerInstance;

		  private string caseExecutionId;

		  public HelperCaseCommandAnonymousInnerClass(CmmnProcessEngineTestCase outerInstance, string caseExecutionId) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.caseExecutionId = caseExecutionId;
		  }

		  public override void execute()
		  {
			getExecution(caseExecutionId).exit();
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void manualStart(final String caseExecutionId)
	  protected internal virtual void manualStart(string caseExecutionId)
	  {
		caseService.withCaseExecution(caseExecutionId).manualStart();
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void occur(final String caseExecutionId)
	  protected internal virtual void occur(string caseExecutionId)
	  {
		executeHelperCaseCommand(new HelperCaseCommandAnonymousInnerClass2(this, caseExecutionId));
	  }

	  private class HelperCaseCommandAnonymousInnerClass2 : HelperCaseCommand
	  {
		  private readonly CmmnProcessEngineTestCase outerInstance;

		  private string caseExecutionId;

		  public HelperCaseCommandAnonymousInnerClass2(CmmnProcessEngineTestCase outerInstance, string caseExecutionId) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.caseExecutionId = caseExecutionId;
		  }

		  public override void execute()
		  {
			getExecution(caseExecutionId).occur();
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void parentResume(final String caseExecutionId)
	  protected internal virtual void parentResume(string caseExecutionId)
	  {
		executeHelperCaseCommand(new HelperCaseCommandAnonymousInnerClass3(this, caseExecutionId));
	  }

	  private class HelperCaseCommandAnonymousInnerClass3 : HelperCaseCommand
	  {
		  private readonly CmmnProcessEngineTestCase outerInstance;

		  private string caseExecutionId;

		  public HelperCaseCommandAnonymousInnerClass3(CmmnProcessEngineTestCase outerInstance, string caseExecutionId) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.caseExecutionId = caseExecutionId;
		  }

		  public override void execute()
		  {
			getExecution(caseExecutionId).parentResume();
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void parentSuspend(final String caseExecutionId)
	  protected internal virtual void parentSuspend(string caseExecutionId)
	  {
		executeHelperCaseCommand(new HelperCaseCommandAnonymousInnerClass4(this, caseExecutionId));
	  }

	  private class HelperCaseCommandAnonymousInnerClass4 : HelperCaseCommand
	  {
		  private readonly CmmnProcessEngineTestCase outerInstance;

		  private string caseExecutionId;

		  public HelperCaseCommandAnonymousInnerClass4(CmmnProcessEngineTestCase outerInstance, string caseExecutionId) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.caseExecutionId = caseExecutionId;
		  }

		  public override void execute()
		  {
			getExecution(caseExecutionId).parentSuspend();

		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void parentTerminate(final String caseExecutionId)
	  protected internal virtual void parentTerminate(string caseExecutionId)
	  {
		executeHelperCaseCommand(new HelperCaseCommandAnonymousInnerClass5(this, caseExecutionId));
	  }

	  private class HelperCaseCommandAnonymousInnerClass5 : HelperCaseCommand
	  {
		  private readonly CmmnProcessEngineTestCase outerInstance;

		  private string caseExecutionId;

		  public HelperCaseCommandAnonymousInnerClass5(CmmnProcessEngineTestCase outerInstance, string caseExecutionId) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.caseExecutionId = caseExecutionId;
		  }

		  public override void execute()
		  {
			getExecution(caseExecutionId).parentTerminate();
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void reactivate(final String caseExecutionId)
	  protected internal virtual void reactivate(string caseExecutionId)
	  {
		executeHelperCaseCommand(new HelperCaseCommandAnonymousInnerClass6(this, caseExecutionId));
	  }

	  private class HelperCaseCommandAnonymousInnerClass6 : HelperCaseCommand
	  {
		  private readonly CmmnProcessEngineTestCase outerInstance;

		  private string caseExecutionId;

		  public HelperCaseCommandAnonymousInnerClass6(CmmnProcessEngineTestCase outerInstance, string caseExecutionId) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.caseExecutionId = caseExecutionId;
		  }

		  public override void execute()
		  {
			getExecution(caseExecutionId).reactivate();
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void reenable(final String caseExecutionId)
	  protected internal virtual void reenable(string caseExecutionId)
	  {
		caseService.withCaseExecution(caseExecutionId).reenable();
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void resume(final String caseExecutionId)
	  protected internal virtual void resume(string caseExecutionId)
	  {
		executeHelperCaseCommand(new HelperCaseCommandAnonymousInnerClass7(this, caseExecutionId));
	  }

	  private class HelperCaseCommandAnonymousInnerClass7 : HelperCaseCommand
	  {
		  private readonly CmmnProcessEngineTestCase outerInstance;

		  private string caseExecutionId;

		  public HelperCaseCommandAnonymousInnerClass7(CmmnProcessEngineTestCase outerInstance, string caseExecutionId) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.caseExecutionId = caseExecutionId;
		  }

		  public override void execute()
		  {
			getExecution(caseExecutionId).resume();
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void suspend(final String caseExecutionId)
	  protected internal virtual void suspend(string caseExecutionId)
	  {
		executeHelperCaseCommand(new HelperCaseCommandAnonymousInnerClass8(this, caseExecutionId));
	  }

	  private class HelperCaseCommandAnonymousInnerClass8 : HelperCaseCommand
	  {
		  private readonly CmmnProcessEngineTestCase outerInstance;

		  private string caseExecutionId;

		  public HelperCaseCommandAnonymousInnerClass8(CmmnProcessEngineTestCase outerInstance, string caseExecutionId) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.caseExecutionId = caseExecutionId;
		  }

		  public override void execute()
		  {
			getExecution(caseExecutionId).suspend();
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void terminate(final String caseExecutionId)
	  protected internal virtual void terminate(string caseExecutionId)
	  {
		executeHelperCaseCommand(new HelperCaseCommandAnonymousInnerClass9(this, caseExecutionId));
	  }

	  private class HelperCaseCommandAnonymousInnerClass9 : HelperCaseCommand
	  {
		  private readonly CmmnProcessEngineTestCase outerInstance;

		  private string caseExecutionId;

		  public HelperCaseCommandAnonymousInnerClass9(CmmnProcessEngineTestCase outerInstance, string caseExecutionId) : base(outerInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.caseExecutionId = caseExecutionId;
		  }

		  public override void execute()
		  {
			getExecution(caseExecutionId).terminate();
		  }
	  }

	  protected internal virtual void executeHelperCaseCommand(HelperCaseCommand command)
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(command);
	  }

	  protected internal abstract class HelperCaseCommand : Command<Void>
	  {
		  private readonly CmmnProcessEngineTestCase outerInstance;

		  public HelperCaseCommand(CmmnProcessEngineTestCase outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		protected internal virtual CmmnExecution getExecution(string caseExecutionId)
		{
		  return (CmmnExecution) outerInstance.caseService.createCaseExecutionQuery().caseExecutionId(caseExecutionId).singleResult();
		}

		public virtual Void execute(CommandContext commandContext)
		{
		  execute();
		  return null;
		}

		public abstract void execute();

	  }

	}

}