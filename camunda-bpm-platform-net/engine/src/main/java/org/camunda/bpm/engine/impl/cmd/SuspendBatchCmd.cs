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
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using UpdateJobDefinitionSuspensionStateBuilderImpl = org.camunda.bpm.engine.impl.management.UpdateJobDefinitionSuspensionStateBuilderImpl;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;

	public class SuspendBatchCmd : AbstractSetBatchStateCmd
	{

	  public SuspendBatchCmd(string batchId) : base(batchId)
	  {
	  }

	  protected internal override SuspensionState NewSuspensionState
	  {
		  get
		  {
			return org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED;
		  }
	  }

	  protected internal override void checkAccess(CommandChecker checker, BatchEntity batch)
	  {
		checker.checkSuspendBatch(batch);
	  }

	  protected internal override AbstractSetJobDefinitionStateCmd createSetJobDefinitionStateCommand(UpdateJobDefinitionSuspensionStateBuilderImpl builder)
	  {
		return new SuspendJobDefinitionCmd(builder);
	  }

	  protected internal override string UserOperationType
	  {
		  get
		  {
			return org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SUSPEND_BATCH;
		  }
	  }

	}

}