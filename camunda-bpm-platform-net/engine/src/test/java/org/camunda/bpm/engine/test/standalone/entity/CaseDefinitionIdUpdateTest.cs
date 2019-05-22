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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;

	public class CaseDefinitionIdUpdateTest : PluggableProcessEngineTestCase
	{

	  public virtual void testUpdateCaseDefinitionIdInTask()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity caseDefinitionEntity1 = prepareCaseDefinition(java.util.UUID.randomUUID().toString());
		CaseDefinitionEntity caseDefinitionEntity1 = prepareCaseDefinition(System.Guid.randomUUID().ToString());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity caseDefinitionEntity2 = prepareCaseDefinition(java.util.UUID.randomUUID().toString());
		CaseDefinitionEntity caseDefinitionEntity2 = prepareCaseDefinition(System.Guid.randomUUID().ToString());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.TaskEntity task = new org.camunda.bpm.engine.impl.persistence.entity.TaskEntity();
		TaskEntity task = new TaskEntity();
		task.Id = System.Guid.randomUUID().ToString();
		task.CaseDefinitionId = caseDefinitionEntity1.Id;

		createTask(task);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.TaskEntity createdTask = findTask(task.getId());
		TaskEntity createdTask = findTask(task.Id);

		assertThat(createdTask).NotNull;

		task.CaseDefinitionId = caseDefinitionEntity2.Id;

		// when
		update(task);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.TaskEntity updatedTask = findTask(task.getId());
		TaskEntity updatedTask = findTask(task.Id);

		// then
		assertThat(updatedTask.CaseDefinitionId).isEqualTo(caseDefinitionEntity2.Id);

		deleteTask(updatedTask);
		deleteCaseDefinition(caseDefinitionEntity1);
		deleteCaseDefinition(caseDefinitionEntity2);
	  }

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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity createdCaseExecution = findCaseExecution(caseExecutionEntity.getId());
		CaseExecutionEntity createdCaseExecution = findCaseExecution(caseExecutionEntity.Id);

		assertThat(createdCaseExecution).NotNull;

		createdCaseExecution.CaseDefinition = caseDefinitionEntity2;

		assertThat(createdCaseExecution.CaseDefinitionId).isEqualTo(caseDefinitionEntity2.Id);

		// when
		update(createdCaseExecution);

		// then
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity updatedCaseExecution = findCaseExecution(createdCaseExecution.getId());
		CaseExecutionEntity updatedCaseExecution = findCaseExecution(createdCaseExecution.Id);
		assertThat(updatedCaseExecution.CaseDefinitionId).isEqualTo(caseDefinitionEntity2.Id);

		deleteCaseExecution(updatedCaseExecution);
		deleteCaseDefinition(caseDefinitionEntity1);
		deleteCaseDefinition(caseDefinitionEntity2);
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
//ORIGINAL LINE: private org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity findCaseExecution(final String id)
	  private CaseExecutionEntity findCaseExecution(string id)
	  {
		return processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass(this, id));
	  }

	  private class CommandAnonymousInnerClass : Command<CaseExecutionEntity>
	  {
		  private readonly CaseDefinitionIdUpdateTest outerInstance;

		  private string id;

		  public CommandAnonymousInnerClass(CaseDefinitionIdUpdateTest outerInstance, string id)
		  {
			  this.outerInstance = outerInstance;
			  this.id = id;
		  }

		  public CaseExecutionEntity execute(CommandContext commandContext)
		  {
			return commandContext.CaseExecutionManager.findCaseExecutionById(id);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private Void deleteCaseExecution(final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity caseExecutionEntity)
	  private Void deleteCaseExecution(CaseExecutionEntity caseExecutionEntity)
	  {
		return processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass2(this, caseExecutionEntity));
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly CaseDefinitionIdUpdateTest outerInstance;

		  private CaseExecutionEntity caseExecutionEntity;

		  public CommandAnonymousInnerClass2(CaseDefinitionIdUpdateTest outerInstance, CaseExecutionEntity caseExecutionEntity)
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
		processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass3(this, caseExecutionEntity));
	  }

	  private class CommandAnonymousInnerClass3 : Command<Void>
	  {
		  private readonly CaseDefinitionIdUpdateTest outerInstance;

		  private CaseExecutionEntity caseExecutionEntity;

		  public CommandAnonymousInnerClass3(CaseDefinitionIdUpdateTest outerInstance, CaseExecutionEntity caseExecutionEntity)
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
//ORIGINAL LINE: private void update(final org.camunda.bpm.engine.impl.db.DbEntity entity)
	  private void update(DbEntity entity)
	  {
		processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass4(this, entity));
	  }

	  private class CommandAnonymousInnerClass4 : Command<Void>
	  {
		  private readonly CaseDefinitionIdUpdateTest outerInstance;

		  private DbEntity entity;

		  public CommandAnonymousInnerClass4(CaseDefinitionIdUpdateTest outerInstance, DbEntity entity)
		  {
			  this.outerInstance = outerInstance;
			  this.entity = entity;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			commandContext.DbEntityManager.merge(entity);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void createCaseDefinition(final org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity caseDefinitionEntity)
	  private void createCaseDefinition(CaseDefinitionEntity caseDefinitionEntity)
	  {
		processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass5(this, caseDefinitionEntity));
	  }

	  private class CommandAnonymousInnerClass5 : Command<Void>
	  {
		  private readonly CaseDefinitionIdUpdateTest outerInstance;

		  private CaseDefinitionEntity caseDefinitionEntity;

		  public CommandAnonymousInnerClass5(CaseDefinitionIdUpdateTest outerInstance, CaseDefinitionEntity caseDefinitionEntity)
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
		return processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass6(this, caseDefinitionEntity));
	  }

	  private class CommandAnonymousInnerClass6 : Command<Void>
	  {
		  private readonly CaseDefinitionIdUpdateTest outerInstance;

		  private CaseDefinitionEntity caseDefinitionEntity;

		  public CommandAnonymousInnerClass6(CaseDefinitionIdUpdateTest outerInstance, CaseDefinitionEntity caseDefinitionEntity)
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
//ORIGINAL LINE: private void createTask(final org.camunda.bpm.engine.impl.persistence.entity.TaskEntity taskEntity)
	  private void createTask(TaskEntity taskEntity)
	  {
		processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass7(this, taskEntity));
	  }

	  private class CommandAnonymousInnerClass7 : Command<Void>
	  {
		  private readonly CaseDefinitionIdUpdateTest outerInstance;

		  private TaskEntity taskEntity;

		  public CommandAnonymousInnerClass7(CaseDefinitionIdUpdateTest outerInstance, TaskEntity taskEntity)
		  {
			  this.outerInstance = outerInstance;
			  this.taskEntity = taskEntity;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			commandContext.TaskManager.insertTask(taskEntity);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void deleteTask(final org.camunda.bpm.engine.impl.persistence.entity.TaskEntity taskEntity)
	  private void deleteTask(TaskEntity taskEntity)
	  {
		processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass8(this, taskEntity));
	  }

	  private class CommandAnonymousInnerClass8 : Command<Void>
	  {
		  private readonly CaseDefinitionIdUpdateTest outerInstance;

		  private TaskEntity taskEntity;

		  public CommandAnonymousInnerClass8(CaseDefinitionIdUpdateTest outerInstance, TaskEntity taskEntity)
		  {
			  this.outerInstance = outerInstance;
			  this.taskEntity = taskEntity;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			commandContext.TaskManager.delete(taskEntity);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private org.camunda.bpm.engine.impl.persistence.entity.TaskEntity findTask(final String id)
	  private TaskEntity findTask(string id)
	  {
		return processEngineConfiguration.CommandExecutorTxRequiresNew.execute(new CommandAnonymousInnerClass9(this, id));
	  }

	  private class CommandAnonymousInnerClass9 : Command<TaskEntity>
	  {
		  private readonly CaseDefinitionIdUpdateTest outerInstance;

		  private string id;

		  public CommandAnonymousInnerClass9(CaseDefinitionIdUpdateTest outerInstance, string id)
		  {
			  this.outerInstance = outerInstance;
			  this.id = id;
		  }

		  public TaskEntity execute(CommandContext commandContext)
		  {
			return commandContext.TaskManager.findTaskById(id);
		  }
	  }
	}

}