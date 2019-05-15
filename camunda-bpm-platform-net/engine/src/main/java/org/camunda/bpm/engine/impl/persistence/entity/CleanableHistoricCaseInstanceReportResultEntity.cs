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
	using CleanableHistoricCaseInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricCaseInstanceReportResult;

	public class CleanableHistoricCaseInstanceReportResultEntity : CleanableHistoricCaseInstanceReportResult
	{

	  protected internal string caseDefinitionId;
	  protected internal string caseDefinitionKey;
	  protected internal string caseDefinitionName;
	  protected internal int caseDefinitionVersion;
	  protected internal int? historyTimeToLive;
	  protected internal long finishedCaseInstanceCount;
	  protected internal long cleanableCaseInstanceCount;
	  protected internal string tenantId;

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId;
		  }
		  set
		  {
			this.caseDefinitionId = value;
		  }
	  }


	  public virtual string CaseDefinitionKey
	  {
		  get
		  {
			return caseDefinitionKey;
		  }
		  set
		  {
			this.caseDefinitionKey = value;
		  }
	  }


	  public virtual string CaseDefinitionName
	  {
		  get
		  {
			return caseDefinitionName;
		  }
		  set
		  {
			this.caseDefinitionName = value;
		  }
	  }


	  public virtual int CaseDefinitionVersion
	  {
		  get
		  {
			return caseDefinitionVersion;
		  }
		  set
		  {
			this.caseDefinitionVersion = value;
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


	  public virtual long getFinishedCaseInstanceCount()
	  {
		return finishedCaseInstanceCount;
	  }

	  public virtual void setFinishedCaseInstanceCount(long? finishedCaseInstanceCount)
	  {
		this.finishedCaseInstanceCount = finishedCaseInstanceCount.Value;
	  }

	  public virtual long getCleanableCaseInstanceCount()
	  {
		return cleanableCaseInstanceCount;
	  }

	  public virtual void setCleanableCaseInstanceCount(long? cleanableCaseInstanceCount)
	  {
		this.cleanableCaseInstanceCount = cleanableCaseInstanceCount.Value;
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
		return this.GetType().Name + "[caseDefinitionId = " + caseDefinitionId + ", caseDefinitionKey = " + caseDefinitionKey + ", caseDefinitionName = " + caseDefinitionName + ", caseDefinitionVersion = " + caseDefinitionVersion + ", historyTimeToLive = " + historyTimeToLive + ", finishedCaseInstanceCount = " + finishedCaseInstanceCount + ", cleanableCaseInstanceCount = " + cleanableCaseInstanceCount + ", tenantId = " + tenantId + "]";
	  }
	}

}