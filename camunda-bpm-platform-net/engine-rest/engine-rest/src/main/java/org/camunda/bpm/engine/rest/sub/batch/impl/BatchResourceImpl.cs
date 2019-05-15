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
namespace org.camunda.bpm.engine.rest.sub.batch.impl
{

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using SuspensionStateDto = org.camunda.bpm.engine.rest.dto.SuspensionStateDto;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using JobDefinitionSuspensionStateDto = org.camunda.bpm.engine.rest.dto.management.JobDefinitionSuspensionStateDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	public class BatchResourceImpl : BatchResource
	{

	  protected internal ProcessEngine processEngine;
	  protected internal string batchId;

	  public BatchResourceImpl(ProcessEngine processEngine, string batchId)
	  {
		this.processEngine = processEngine;
		this.batchId = batchId;
	  }

	  public virtual BatchDto Batch
	  {
		  get
		  {
			Batch batch = processEngine.ManagementService.createBatchQuery().batchId(batchId).singleResult();
    
			if (batch == null)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, "Batch with id '" + batchId + "' does not exist");
			}
    
			return BatchDto.fromBatch(batch);
		  }
	  }

	  public virtual void updateSuspensionState(SuspensionStateDto suspensionState)
	  {
		if (suspensionState.Suspended)
		{
		  suspendBatch();
		}
		else
		{
		  activateBatch();
		}
	  }

	  protected internal virtual void suspendBatch()
	  {
		try
		{
		  processEngine.ManagementService.suspendBatchById(batchId);
		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, "Unable to suspend batch with id '" + batchId + "'");
		}
	  }

	  protected internal virtual void activateBatch()
	  {
		try
		{
		  processEngine.ManagementService.activateBatchById(batchId);
		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, "Unable to activate batch with id '" + batchId + "'");
		}
	  }

	  public virtual void deleteBatch(bool cascade)
	  {
		try
		{
		  processEngine.ManagementService.deleteBatch(batchId, cascade);
		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, "Unable to delete batch with id '" + batchId + "'");
		}
	  }

	}

}