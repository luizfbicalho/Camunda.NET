using System;

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
namespace org.camunda.bpm.engine.test.standalone.entity
{
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using HistoricCaseActivityInstanceEventEntity = org.camunda.bpm.engine.impl.history.@event.HistoricCaseActivityInstanceEventEntity;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using HistoricCaseActivityInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricCaseActivityInstanceEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

	public class CaseDefinitionIdHistoryUpdateTest : PluggableProcessEngineTestCase
	{

	  public virtual void testUpdateCaseDefinitionIdInCaseExecutionEntity()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity caseDefinitionEntity1 = prepareCaseDefinition(java.util.UUID.randomUUID().toString());
		CaseDefinitionEntity caseDefinitionEntity1 = prepareCaseDefinition(System.Guid.randomUUID().ToString());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity caseDefinitionEntity2 = prepareCaseDefinition(java.util.UUID.randomUUID().toString());
		CaseDefinitionEntity caseDefinitionEntity2 = prepareCaseDefinition(System.Guid.randomUUID().ToString());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity caseExecutionEntity = prepareCaseExecution(caseDefinitionEntity1);
		CaseExecutionEntity caseExecutionEntity = prepareCaseExecution(caseDefinitionEntity1);

		assertThat(caseExecutionEntity.CaseDefinitionId).isEqualTo(caseDefinitionEntity1.Id);

		createCaseExecution(caseExecutionEntity);

		caseExecutionEntity.CaseDefinition = caseDefinitionEntity2;

		// Create
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.history.event.HistoricCaseActivityInstanceEventEntity historicCaseActivityInstanceEntity = prepareHistoricCaseActivityInstance(caseDefinitionEntity1);
		HistoricCaseActivityInstanceEventEntity historicCaseActivityInstanceEntity = prepareHistoricCaseActivityInstance(caseDefinitionEntity1);
		createCaseExecutionHistory(historicCaseActivityInstanceEntity);

		// when
		// Set new caseDefinitionId and update
		historicCaseActivityInstanceEntity.CaseDefinitionId = caseDefinitionEntity2.Id;
		historicCaseActivityInstanceEntity.EventType = HistoryEventTypes.CASE_ACTIVITY_INSTANCE_UPDATE.EventName;
		updateCaseExecutionHistory(historicCaseActivityInstanceEntity);

		// then
		// Read from DB and assert
		HistoricCaseActivityInstanceEntity updatedInstance = findHistoricCaseActivityInstance(historicCaseActivityInstanceEntity.Id);
		assertThat(updatedInstance.CaseDefinitionId).isEqualTo(caseDefinitionEntity2.Id);

		deleteHistoricCaseActivityInstance(historicCaseActivityInstanceEntity);
		deleteCaseExecution(caseExecutionEntity);
		deleteCaseDefinition(caseDefinitionEntity1);
		deleteCaseDefinition(caseDefinitionEntity2);
	  }

	  private HistoricCaseActivityInstanceEventEntity prepareHistoricCaseActivityInstance(CaseDefinitionEntity caseDefinitionEntity1)
	  {
		HistoricCaseActivityInstanceEventEntity historicCaseActivityInstanceEntity = new HistoricCaseActivityInstanceEventEntity();
		historicCaseActivityInstanceEntity.Id = System.Guid.randomUUID().ToString();
		historicCaseActivityInstanceEntity.CaseDefinitionId = caseDefinitionEntity1.Id;
		historicCaseActivityInstanceEntity.CaseInstanceId = System.Guid.randomUUID().ToString();
		historicCaseActivityInstanceEntity.CaseActivityId = System.Guid.randomUUID().ToString();
		historicCaseActivityInstanceEntity.CreateTime = DateTime.Now;
		return historicCaseActivityInstanceEntity;
	  }

	  private CaseExecutionEntity prepareCaseExecution(CaseDefinitionEntity caseDefinitionEntity1)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity caseExecutionEntity = new org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity();
		CaseExecutionEntity caseExecutionEntity = new CaseExecutionEntity();
		caseExecutionEntity.Id = System.Guid.randomUUID().ToString();
		caseExecutionEntity.CaseDefinition = caseDefinitionEntity1;
		return caseExecutionEntity;
	  }

	  private CaseDefinitionEntity prepareCaseDefinition(string id)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity caseDefinitionEntity = new org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity();
		CaseDefinitionEntity caseDefinitionEntity = new CaseDefinitionEntity();
		caseDefinitionEntity.Id = id;
		caseDefinitionEntity.Key = System.Guid.randomUUID().ToString();
		caseDefinitionEntity.DeploymentId = System.Guid.randomUUID().ToString();
		createCaseDefinition(caseDefinitionEntity);
		return caseDefinitionEntity;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private Void createCaseExecutionHistory(final org.camunda.bpm.engine.impl.history.event.HistoricCaseActivityInstanceEventEntity historicCaseActivityInstanceEntity)
	  private Void createCaseExecutionHistory(HistoricCaseActivityInstanceEventEntity historicCaseActivityInstanceEntity)
	  {
		return processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass(this, historicCaseActivityInstanceEntity));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly CaseDefinitionIdHistoryUpdateTest outerInstance;

		  private HistoricCaseActivityInstanceEventEntity historicCaseActivityInstanceEntity;

		  public CommandAnonymousInnerClass(CaseDefinitionIdHistoryUpdateTest outerInstance, HistoricCaseActivityInstanceEventEntity historicCaseActivityInstanceEntity)
		  {
			  this.outerInstance = outerInstance;
			  this.historicCaseActivityInstanceEntity = historicCaseActivityInstanceEntity;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			commandContext.DbEntityManager.insert(historicCaseActivityInstanceEntity);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private Void updateCaseExecutionHistory(final org.camunda.bpm.engine.impl.history.event.HistoricCaseActivityInstanceEventEntity historicCaseActivityInstanceEntity)
	  private Void updateCaseExecutionHistory(HistoricCaseActivityInstanceEventEntity historicCaseActivityInstanceEntity)
	  {
		return processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass2(this, historicCaseActivityInstanceEntity));
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly CaseDefinitionIdHistoryUpdateTest outerInstance;

		  private HistoricCaseActivityInstanceEventEntity historicCaseActivityInstanceEntity;

		  public CommandAnonymousInnerClass2(CaseDefinitionIdHistoryUpdateTest outerInstance, HistoricCaseActivityInstanceEventEntity historicCaseActivityInstanceEntity)
		  {
			  this.outerInstance = outerInstance;
			  this.historicCaseActivityInstanceEntity = historicCaseActivityInstanceEntity;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			commandContext.DbEntityManager.merge(historicCaseActivityInstanceEntity);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private org.camunda.bpm.engine.impl.persistence.entity.HistoricCaseActivityInstanceEntity findHistoricCaseActivityInstance(final String id)
	  private HistoricCaseActivityInstanceEntity findHistoricCaseActivityInstance(string id)
	  {
		return processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass3(this, id));
	  }

	  private class CommandAnonymousInnerClass3 : Command<HistoricCaseActivityInstanceEntity>
	  {
		  private readonly CaseDefinitionIdHistoryUpdateTest outerInstance;

		  private string id;

		  public CommandAnonymousInnerClass3(CaseDefinitionIdHistoryUpdateTest outerInstance, string id)
		  {
			  this.outerInstance = outerInstance;
			  this.id = id;
		  }

		  public HistoricCaseActivityInstanceEntity execute(CommandContext commandContext)
		  {
			return (HistoricCaseActivityInstanceEntity) commandContext.DbEntityManager.selectOne("selectHistoricCaseActivityInstance", id);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private Void deleteCaseExecution(final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity caseExecutionEntity)
	  private Void deleteCaseExecution(CaseExecutionEntity caseExecutionEntity)
	  {
		return processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass4(this, caseExecutionEntity));
	  }

	  private class CommandAnonymousInnerClass4 : Command<Void>
	  {
		  private readonly CaseDefinitionIdHistoryUpdateTest outerInstance;

		  private CaseExecutionEntity caseExecutionEntity;

		  public CommandAnonymousInnerClass4(CaseDefinitionIdHistoryUpdateTest outerInstance, CaseExecutionEntity caseExecutionEntity)
		  {
			  this.outerInstance = outerInstance;
			  this.caseExecutionEntity = caseExecutionEntity;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			commandContext.CaseExecutionManager.deleteCaseExecution(caseExecutionEntity);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void createCaseExecution(final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity caseExecutionEntity)
	  private void createCaseExecution(CaseExecutionEntity caseExecutionEntity)
	  {
		processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass5(this, caseExecutionEntity));
	  }

	  private class CommandAnonymousInnerClass5 : Command<Void>
	  {
		  private readonly CaseDefinitionIdHistoryUpdateTest outerInstance;

		  private CaseExecutionEntity caseExecutionEntity;

		  public CommandAnonymousInnerClass5(CaseDefinitionIdHistoryUpdateTest outerInstance, CaseExecutionEntity caseExecutionEntity)
		  {
			  this.outerInstance = outerInstance;
			  this.caseExecutionEntity = caseExecutionEntity;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			commandContext.CaseExecutionManager.insertCaseExecution(caseExecutionEntity);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void createCaseDefinition(final org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity caseDefinitionEntity)
	  private void createCaseDefinition(CaseDefinitionEntity caseDefinitionEntity)
	  {
		processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass6(this, caseDefinitionEntity));
	  }

	  private class CommandAnonymousInnerClass6 : Command<Void>
	  {
		  private readonly CaseDefinitionIdHistoryUpdateTest outerInstance;

		  private CaseDefinitionEntity caseDefinitionEntity;

		  public CommandAnonymousInnerClass6(CaseDefinitionIdHistoryUpdateTest outerInstance, CaseDefinitionEntity caseDefinitionEntity)
		  {
			  this.outerInstance = outerInstance;
			  this.caseDefinitionEntity = caseDefinitionEntity;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			commandContext.CaseDefinitionManager.insertCaseDefinition(caseDefinitionEntity);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private Void deleteCaseDefinition(final org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity caseDefinitionEntity)
	  private Void deleteCaseDefinition(CaseDefinitionEntity caseDefinitionEntity)
	  {
		return processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass7(this, caseDefinitionEntity));
	  }

	  private class CommandAnonymousInnerClass7 : Command<Void>
	  {
		  private readonly CaseDefinitionIdHistoryUpdateTest outerInstance;

		  private CaseDefinitionEntity caseDefinitionEntity;

		  public CommandAnonymousInnerClass7(CaseDefinitionIdHistoryUpdateTest outerInstance, CaseDefinitionEntity caseDefinitionEntity)
		  {
			  this.outerInstance = outerInstance;
			  this.caseDefinitionEntity = caseDefinitionEntity;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			commandContext.CaseDefinitionManager.deleteCaseDefinitionsByDeploymentId(caseDefinitionEntity.DeploymentId);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private Void deleteHistoricCaseActivityInstance(final org.camunda.bpm.engine.impl.history.event.HistoricCaseActivityInstanceEventEntity entity)
	  private Void deleteHistoricCaseActivityInstance(HistoricCaseActivityInstanceEventEntity entity)
	  {
		return processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass8(this, entity));
	  }

	  private class CommandAnonymousInnerClass8 : Command<Void>
	  {
		  private readonly CaseDefinitionIdHistoryUpdateTest outerInstance;

		  private HistoricCaseActivityInstanceEventEntity entity;

		  public CommandAnonymousInnerClass8(CaseDefinitionIdHistoryUpdateTest outerInstance, HistoricCaseActivityInstanceEventEntity entity)
		  {
			  this.outerInstance = outerInstance;
			  this.entity = entity;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			commandContext.HistoricCaseActivityInstanceManager.deleteHistoricCaseActivityInstancesByCaseInstanceIds(singletonList(entity.CaseInstanceId));
			return null;
		  }
	  }
	}

}