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
	using CleanableHistoricProcessInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReportResult;


	public class CleanableHistoricProcessInstanceReportResultEntity : CleanableHistoricProcessInstanceReportResult
	{

	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string processDefinitionName;
	  protected internal int processDefinitionVersion;
	  protected internal int? historyTimeToLive;
	  protected internal long finishedProcessInstanceCount;
	  protected internal long cleanableProcessInstanceCount;
	  protected internal string tenantId;

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }


	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
		  set
		  {
			this.processDefinitionKey = value;
		  }
	  }


	  public virtual string ProcessDefinitionName
	  {
		  get
		  {
			return processDefinitionName;
		  }
		  set
		  {
			this.processDefinitionName = value;
		  }
	  }


	  public virtual int ProcessDefinitionVersion
	  {
		  get
		  {
			return processDefinitionVersion;
		  }
		  set
		  {
			this.processDefinitionVersion = value;
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


	  public virtual long getFinishedProcessInstanceCount()
	  {
		return finishedProcessInstanceCount;
	  }

	  public virtual void setFinishedProcessInstanceCount(long? finishedProcessInstanceCount)
	  {
		this.finishedProcessInstanceCount = finishedProcessInstanceCount.Value;
	  }

	  public virtual long getCleanableProcessInstanceCount()
	  {
		return cleanableProcessInstanceCount;
	  }

	  public virtual void setCleanableProcessInstanceCount(long? cleanableProcessInstanceCount)
	  {
		this.cleanableProcessInstanceCount = cleanableProcessInstanceCount.Value;
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
		  set
		  {
			this.tenantId = value;
		  }
	  }


	  public override string ToString()
	  {
		return this.GetType().Name + "[processDefinitionId = " + processDefinitionId + ", processDefinitionKey = " + processDefinitionKey + ", processDefinitionName = " + processDefinitionName + ", processDefinitionVersion = " + processDefinitionVersion + ", historyTimeToLive = " + historyTimeToLive + ", finishedProcessInstanceCount = " + finishedProcessInstanceCount + ", cleanableProcessInstanceCount = " + cleanableProcessInstanceCount + ", tenantId = " + tenantId + "]";
	  }
	}

}