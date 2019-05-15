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
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.AVAILABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.COMPLETED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.DISABLED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ENABLED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.SUSPENDED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.TERMINATED;


	/// <summary>
	/// <para><seealso cref="HistoryEvent"/> implementation for events that happen in a case activity.</para>
	/// 
	/// @author Sebastian Menski
	/// </summary>
	[Serializable]
	public class HistoricCaseActivityInstanceEventEntity : HistoricScopeInstanceEvent
	{

	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// the id of the case activity </summary>
	  protected internal string caseActivityId;

	  /// <summary>
	  /// the name of the case activity </summary>
	  protected internal string caseActivityName;

	  /// <summary>
	  /// the type of the case activity </summary>
	  protected internal string caseActivityType;

	  /// <summary>
	  /// the state of this case activity instance </summary>
	  protected internal int caseActivityInstanceState;

	  /// <summary>
	  /// the id of the parent case activity instance </summary>
	  protected internal string parentCaseActivityInstanceId;

	  /// <summary>
	  /// the id of the called task in case of a human task </summary>
	  protected internal string taskId;

	  /// <summary>
	  /// the id of the called process in case of a process task </summary>
	  protected internal string calledProcessInstanceId;

	  /// <summary>
	  /// the id of the called case in case of a case task </summary>
	  protected internal string calledCaseInstanceId;

	  /// <summary>
	  /// id of the tenant which belongs to the case activity instance </summary>
	  protected internal string tenantId;

	  /// <summary>
	  /// the flag whether this case activity is required </summary>
	  protected internal bool required = false;

	  // getters and setters //////////////////////////////////////////////////////

	  public override string CaseExecutionId
	  {
		  get
		  {
			return Id;
		  }
	  }

	  public virtual string CaseActivityId
	  {
		  get
		  {
			return caseActivityId;
		  }
		  set
		  {
			this.caseActivityId = value;
		  }
	  }


	  public virtual string CaseActivityName
	  {
		  get
		  {
			return caseActivityName;
		  }
		  set
		  {
			this.caseActivityName = value;
		  }
	  }


	  public virtual string CaseActivityType
	  {
		  get
		  {
			return caseActivityType;
		  }
		  set
		  {
			this.caseActivityType = value;
		  }
	  }


	  public virtual int CaseActivityInstanceState
	  {
		  get
		  {
			return caseActivityInstanceState;
		  }
		  set
		  {
			this.caseActivityInstanceState = value;
		  }
	  }


	  public virtual string ParentCaseActivityInstanceId
	  {
		  get
		  {
			return parentCaseActivityInstanceId;
		  }
		  set
		  {
			this.parentCaseActivityInstanceId = value;
		  }
	  }


	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
		  set
		  {
			this.taskId = value;
		  }
	  }


	  public virtual string CalledProcessInstanceId
	  {
		  get
		  {
			return calledProcessInstanceId;
		  }
		  set
		  {
			this.calledProcessInstanceId = value;
		  }
	  }


	  public virtual string CalledCaseInstanceId
	  {
		  get
		  {
			return calledCaseInstanceId;
		  }
		  set
		  {
			this.calledCaseInstanceId = value;
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


	  public virtual DateTime CreateTime
	  {
		  get
		  {
			return startTime;
		  }
		  set
		  {
			StartTime = value;
		  }
	  }


	  public virtual bool Required
	  {
		  get
		  {
			return required;
		  }
		  set
		  {
			this.required = value;
		  }
	  }


	  public virtual bool Available
	  {
		  get
		  {
			return caseActivityInstanceState == AVAILABLE.StateCode;
		  }
	  }

	  public virtual bool Enabled
	  {
		  get
		  {
			return caseActivityInstanceState == ENABLED.StateCode;
		  }
	  }

	  public virtual bool Disabled
	  {
		  get
		  {
			return caseActivityInstanceState == DISABLED.StateCode;
		  }
	  }

	  public virtual bool Active
	  {
		  get
		  {
			return caseActivityInstanceState == ACTIVE.StateCode;
		  }
	  }

	  public virtual bool Suspended
	  {
		  get
		  {
			return caseActivityInstanceState == SUSPENDED.StateCode;
		  }
	  }

	  public virtual bool Completed
	  {
		  get
		  {
			return caseActivityInstanceState == COMPLETED.StateCode;
		  }
	  }

	  public virtual bool Terminated
	  {
		  get
		  {
			return caseActivityInstanceState == TERMINATED.StateCode;
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[caseActivityId=" + caseActivityId + ", caseActivityName=" + caseActivityName + ", caseActivityInstanceId=" + id + ", caseActivityInstanceState=" + caseActivityInstanceState + ", parentCaseActivityInstanceId=" + parentCaseActivityInstanceId + ", taskId=" + taskId + ", calledProcessInstanceId=" + calledProcessInstanceId + ", calledCaseInstanceId=" + calledCaseInstanceId + ", durationInMillis=" + durationInMillis + ", createTime=" + startTime + ", endTime=" + endTime + ", eventType=" + eventType + ", caseExecutionId=" + caseExecutionId + ", caseDefinitionId=" + caseDefinitionId + ", caseInstanceId=" + caseInstanceId + ", tenantId=" + tenantId + "]";
	  }
	}

}