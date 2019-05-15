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
namespace org.camunda.bpm.engine.rest.sub.history.impl
{

	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using HistoricBatchDto = org.camunda.bpm.engine.rest.dto.history.batch.HistoricBatchDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	public class HistoricBatchResourceImpl : HistoricBatchResource
	{

	  protected internal ProcessEngine processEngine;
	  protected internal string batchId;

	  public HistoricBatchResourceImpl(ProcessEngine processEngine, string batchId)
	  {
		this.processEngine = processEngine;
		this.batchId = batchId;
	  }

	  public virtual HistoricBatchDto HistoricBatch
	  {
		  get
		  {
			HistoricBatch batch = processEngine.HistoryService.createHistoricBatchQuery().batchId(batchId).singleResult();
    
			if (batch == null)
			{
			  throw new InvalidRequestException(Status.NOT_FOUND, "Historic batch with id '" + batchId + "' does not exist");
			}
    
			return HistoricBatchDto.fromBatch(batch);
		  }
	  }

	  public virtual void deleteHistoricBatch()
	  {
		try
		{
		  processEngine.HistoryService.deleteHistoricBatch(batchId);
		}
		catch (BadUserRequestException e)
		{
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, "Unable to delete historic batch with id '" + batchId + "'");
		}
	  }

	}

}