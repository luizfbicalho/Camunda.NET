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
namespace org.camunda.bpm.engine.impl.history.@event
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ACTIVE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.CLOSED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.COMPLETED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.FAILED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.SUSPENDED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.TERMINATED;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	[Serializable]
	public class HistoricCaseInstanceEventEntity : HistoricScopeInstanceEvent
	{

	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// the business key of the case instance </summary>
	  protected internal string businessKey;

	  /// <summary>
	  /// the id of the user that created the case instance </summary>
	  protected internal string createUserId;

	  /// <summary>
	  /// the current state of the case instance </summary>
	  protected internal int state;

	  /// <summary>
	  /// the case instance which started this case instance </summary>
	  protected internal string superCaseInstanceId;

	  /// <summary>
	  /// the process instance which started this case instance </summary>
	  protected internal string superProcessInstanceId;

	  /// <summary>
	  /// id of the tenant which belongs to the case instance </summary>
	  protected internal string tenantId;

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
		  set
		  {
			this.businessKey = value;
		  }
	  }


	  public virtual DateTime CreateTime
	  {
		  get
		  {
			return StartTime;
		  }
		  set
		  {
			StartTime = value;
		  }
	  }


	  public virtual DateTime CloseTime
	  {
		  get
		  {
			return EndTime;
		  }
		  set
		  {
			EndTime = value;
		  }
	  }


	  public virtual string CreateUserId
	  {
		  get
		  {
			return createUserId;
		  }
		  set
		  {
			createUserId = value;
		  }
	  }


	  public virtual string SuperCaseInstanceId
	  {
		  get
		  {
			return superCaseInstanceId;
		  }
		  set
		  {
			this.superCaseInstanceId = value;
		  }
	  }


	  public virtual string SuperProcessInstanceId
	  {
		  get
		  {
			return superProcessInstanceId;
		  }
		  set
		  {
			this.superProcessInstanceId = value;
		  }
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


	  public virtual int State
	  {
		  get
		  {
			return state;
		  }
		  set
		  {
			this.state = value;
		  }
	  }


	  public virtual bool Active
	  {
		  get
		  {
			return state == ACTIVE.StateCode;
		  }
	  }

	  public virtual bool Completed
	  {
		  get
		  {
			return state == COMPLETED.StateCode;
		  }
	  }

	  public virtual bool Terminated
	  {
		  get
		  {
			return state == TERMINATED.StateCode;
		  }
	  }

	  public virtual bool Failed
	  {
		  get
		  {
			return state == FAILED.StateCode;
		  }
	  }

	  public virtual bool Suspended
	  {
		  get
		  {
			return state == SUSPENDED.StateCode;
		  }
	  }

	  public virtual bool Closed
	  {
		  get
		  {
			return state == CLOSED.StateCode;
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[businessKey=" + businessKey + ", startUserId=" + createUserId + ", superCaseInstanceId=" + superCaseInstanceId + ", superProcessInstanceId=" + superProcessInstanceId + ", durationInMillis=" + durationInMillis + ", createTime=" + startTime + ", closeTime=" + endTime + ", id=" + id + ", eventType=" + eventType + ", caseExecutionId=" + caseExecutionId + ", caseDefinitionId=" + caseDefinitionId + ", caseInstanceId=" + caseInstanceId + ", tenantId=" + tenantId + "]";
	  }
	}

}