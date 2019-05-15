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
namespace org.camunda.bpm.engine.impl.persistence.entity
{
	using CleanableHistoricBatchReportResult = org.camunda.bpm.engine.history.CleanableHistoricBatchReportResult;

	public class CleanableHistoricBatchesReportResultEntity : CleanableHistoricBatchReportResult
	{

	  protected internal string batchType;
	  protected internal int? historyTimeToLive;
	  protected internal long finishedBatchesCount;
	  protected internal long cleanableBatchesCount;

	  public virtual string BatchType
	  {
		  get
		  {
			return batchType;
		  }
		  set
		  {
			this.batchType = value;
		  }
	  }


	  public virtual int? HistoryTimeToLive
	  {
		  get
		  {
			return historyTimeToLive;
		  }
		  set
		  {
			this.historyTimeToLive = value;
		  }
	  }


	  public virtual long FinishedBatchesCount
	  {
		  get
		  {
			return finishedBatchesCount;
		  }
		  set
		  {
			this.finishedBatchesCount = value;
		  }
	  }


	  public virtual long CleanableBatchesCount
	  {
		  get
		  {
			return cleanableBatchesCount;
		  }
		  set
		  {
			this.cleanableBatchesCount = value;
		  }
	  }


	  public override string ToString()
	  {
		return this.GetType().Name + "[batchType = " + batchType + ", historyTimeToLive = " + historyTimeToLive + ", finishedBatchesCount = " + finishedBatchesCount + ", cleanableBatchesCount = " + cleanableBatchesCount + "]";
	  }
	}

}