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
namespace org.camunda.bpm.engine.impl
{
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using OptimisticLockingListener = org.camunda.bpm.engine.impl.db.entitymanager.OptimisticLockingListener;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using EverLivingJobEntity = org.camunda.bpm.engine.impl.persistence.entity.EverLivingJobEntity;
	using PropertyEntity = org.camunda.bpm.engine.impl.persistence.entity.PropertyEntity;

	/// <summary>
	/// @author Nikola Koevski
	/// </summary>
	public class BootstrapEngineCommand : ProcessEngineBootstrapCommand
	{


	  private static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  public override Void execute(CommandContext commandContext)
	  {

		checkDeploymentLockExists(commandContext);
		checkHistoryCleanupLockExists(commandContext);
		createHistoryCleanupJob(commandContext);

		return null;
	  }

	  protected internal virtual void createHistoryCleanupJob(CommandContext commandContext)
	  {
		if (Context.ProcessEngineConfiguration.ManagementService.getTableMetaData("ACT_RU_JOB") != null)
		{
		  // CAM-9671: avoid transaction rollback due to the OLE being caught in CommandContext#close
		  commandContext.DbEntityManager.registerOptimisticLockingListener(new OptimisticLockingListenerAnonymousInnerClass(this));
		  Context.ProcessEngineConfiguration.HistoryService.cleanUpHistoryAsync();
		}
	  }

	  private class OptimisticLockingListenerAnonymousInnerClass : OptimisticLockingListener
	  {
		  private readonly BootstrapEngineCommand outerInstance;

		  public OptimisticLockingListenerAnonymousInnerClass(BootstrapEngineCommand outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public Type EntityType
		  {
			  get
			  {
				return typeof(EverLivingJobEntity);
			  }
		  }

		  public void failedOperation(DbOperation operation)
		  {
			// nothing do to, reconfiguration will be handled later on
		  }
	  }

	  public virtual void checkDeploymentLockExists(CommandContext commandContext)
	  {
		PropertyEntity deploymentLockProperty = commandContext.PropertyManager.findPropertyById("deployment.lock");
		if (deploymentLockProperty == null)
		{
		  LOG.noDeploymentLockPropertyFound();
		}
	  }

	  public virtual void checkHistoryCleanupLockExists(CommandContext commandContext)
	  {
		PropertyEntity historyCleanupLockProperty = commandContext.PropertyManager.findPropertyById("history.cleanup.job.lock");
		if (historyCleanupLockProperty == null)
		{
		  LOG.noHistoryCleanupLockPropertyFound();
		}
	  }
	}

}