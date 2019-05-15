using System.Collections.Generic;

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
	/// <summary>
	/// The set of built-in history event types.
	/// 
	/// @author Daniel Meyer
	/// @author Ingo Richtsmeier
	/// @since 7.2
	/// </summary>
	public sealed class HistoryEventTypes : HistoryEventType
	{

	  /// <summary>
	  /// fired when a process instance is started. </summary>
	  public static readonly HistoryEventTypes PROCESS_INSTANCE_START = new HistoryEventTypes("PROCESS_INSTANCE_START", InnerEnum.PROCESS_INSTANCE_START, "process-instance", "start");
	  /// <summary>
	  /// fired when a process instance is updated </summary>
	  public static readonly HistoryEventTypes PROCESS_INSTANCE_UPDATE = new HistoryEventTypes("PROCESS_INSTANCE_UPDATE", InnerEnum.PROCESS_INSTANCE_UPDATE, "process-instance-update", "update");
	  /// <summary>
	  /// fired when a process instance is migrated </summary>
	  public static readonly HistoryEventTypes PROCESS_INSTANCE_MIGRATE = new HistoryEventTypes("PROCESS_INSTANCE_MIGRATE", InnerEnum.PROCESS_INSTANCE_MIGRATE, "process-instance", "migrate");
	  /// <summary>
	  /// fired when a process instance is ended. </summary>
	  public static readonly HistoryEventTypes PROCESS_INSTANCE_END = new HistoryEventTypes("PROCESS_INSTANCE_END", InnerEnum.PROCESS_INSTANCE_END, "process-instance", "end");

	  /// <summary>
	  /// fired when an activity instance is started. </summary>
	  public static readonly HistoryEventTypes ACTIVITY_INSTANCE_START = new HistoryEventTypes("ACTIVITY_INSTANCE_START", InnerEnum.ACTIVITY_INSTANCE_START, "activity-instance", "start");
	  /// <summary>
	  /// fired when an activity instance is updated. </summary>
	  public static readonly HistoryEventTypes ACTIVITY_INSTANCE_UPDATE = new HistoryEventTypes("ACTIVITY_INSTANCE_UPDATE", InnerEnum.ACTIVITY_INSTANCE_UPDATE, "activity-instance", "update");
	  /// <summary>
	  /// fired when an activity instance is migrated. </summary>
	  public static readonly HistoryEventTypes ACTIVITY_INSTANCE_MIGRATE = new HistoryEventTypes("ACTIVITY_INSTANCE_MIGRATE", InnerEnum.ACTIVITY_INSTANCE_MIGRATE, "activity-instance", "migrate");
	  /// <summary>
	  /// fired when an activity instance is ended. </summary>
	  public static readonly HistoryEventTypes ACTIVITY_INSTANCE_END = new HistoryEventTypes("ACTIVITY_INSTANCE_END", InnerEnum.ACTIVITY_INSTANCE_END, "activity-instance", "end");

	  /// <summary>
	  /// fired when a task instance is created. </summary>
	  public static readonly HistoryEventTypes TASK_INSTANCE_CREATE = new HistoryEventTypes("TASK_INSTANCE_CREATE", InnerEnum.TASK_INSTANCE_CREATE, "task-instance", "create");
	  /// <summary>
	  /// fired when a task instance is updated. </summary>
	  public static readonly HistoryEventTypes TASK_INSTANCE_UPDATE = new HistoryEventTypes("TASK_INSTANCE_UPDATE", InnerEnum.TASK_INSTANCE_UPDATE, "task-instance", "update");
	  /// <summary>
	  /// fired when a task instance is migrated. </summary>
	  public static readonly HistoryEventTypes TASK_INSTANCE_MIGRATE = new HistoryEventTypes("TASK_INSTANCE_MIGRATE", InnerEnum.TASK_INSTANCE_MIGRATE, "task-instance", "migrate");
	  /// <summary>
	  /// fired when a task instance is completed. </summary>
	  public static readonly HistoryEventTypes TASK_INSTANCE_COMPLETE = new HistoryEventTypes("TASK_INSTANCE_COMPLETE", InnerEnum.TASK_INSTANCE_COMPLETE, "task-instance", "complete");
	  /// <summary>
	  /// fired when a task instance is deleted. </summary>
	  public static readonly HistoryEventTypes TASK_INSTANCE_DELETE = new HistoryEventTypes("TASK_INSTANCE_DELETE", InnerEnum.TASK_INSTANCE_DELETE, "task-instance", "delete");

	  /// <summary>
	  /// fired when a variable instance is created. </summary>
	  public static readonly HistoryEventTypes VARIABLE_INSTANCE_CREATE = new HistoryEventTypes("VARIABLE_INSTANCE_CREATE", InnerEnum.VARIABLE_INSTANCE_CREATE, "variable-instance", "create");
	  /// <summary>
	  /// fired when a variable instance is updated. </summary>
	  public static readonly HistoryEventTypes VARIABLE_INSTANCE_UPDATE = new HistoryEventTypes("VARIABLE_INSTANCE_UPDATE", InnerEnum.VARIABLE_INSTANCE_UPDATE, "variable-instance", "update");
	  /// <summary>
	  /// fired when a variable instance is migrated. </summary>
	  public static readonly HistoryEventTypes VARIABLE_INSTANCE_MIGRATE = new HistoryEventTypes("VARIABLE_INSTANCE_MIGRATE", InnerEnum.VARIABLE_INSTANCE_MIGRATE, "variable-instance", "migrate");
	  /// <summary>
	  /// fired when a variable instance is updated. </summary>
	  public static readonly HistoryEventTypes VARIABLE_INSTANCE_UPDATE_DETAIL = new HistoryEventTypes("VARIABLE_INSTANCE_UPDATE_DETAIL", InnerEnum.VARIABLE_INSTANCE_UPDATE_DETAIL, "variable-instance", "update-detail");
	  /// <summary>
	  /// fired when a variable instance is deleted. </summary>
	  public static readonly HistoryEventTypes VARIABLE_INSTANCE_DELETE = new HistoryEventTypes("VARIABLE_INSTANCE_DELETE", InnerEnum.VARIABLE_INSTANCE_DELETE, "variable-instance", "delete");

	  /// <summary>
	  /// fired when a form property is updated. </summary>
	  public static readonly HistoryEventTypes FORM_PROPERTY_UPDATE = new HistoryEventTypes("FORM_PROPERTY_UPDATE", InnerEnum.FORM_PROPERTY_UPDATE, "form-property", "form-property-update");

	  /// <summary>
	  /// fired when an incident is created. </summary>
	  public static readonly HistoryEventTypes INCIDENT_CREATE = new HistoryEventTypes("INCIDENT_CREATE", InnerEnum.INCIDENT_CREATE, "incident", "create");
	  /// <summary>
	  /// fired when an incident is migrated. </summary>
	  public static readonly HistoryEventTypes INCIDENT_MIGRATE = new HistoryEventTypes("INCIDENT_MIGRATE", InnerEnum.INCIDENT_MIGRATE, "incident", "migrate");
	  /// <summary>
	  /// fired when an incident is deleted. </summary>
	  public static readonly HistoryEventTypes INCIDENT_DELETE = new HistoryEventTypes("INCIDENT_DELETE", InnerEnum.INCIDENT_DELETE, "incident", "delete");
	  /// <summary>
	  /// fired when an incident is resolved. </summary>
	  public static readonly HistoryEventTypes INCIDENT_RESOLVE = new HistoryEventTypes("INCIDENT_RESOLVE", InnerEnum.INCIDENT_RESOLVE, "incident", "resolve");

	  /// <summary>
	  /// fired when a case instance is created. </summary>
	  public static readonly HistoryEventTypes CASE_INSTANCE_CREATE = new HistoryEventTypes("CASE_INSTANCE_CREATE", InnerEnum.CASE_INSTANCE_CREATE, "case-instance", "create");
	  /// <summary>
	  /// fired when a case instance is updated. </summary>
	  public static readonly HistoryEventTypes CASE_INSTANCE_UPDATE = new HistoryEventTypes("CASE_INSTANCE_UPDATE", InnerEnum.CASE_INSTANCE_UPDATE, "case-instance", "update");
	  /// <summary>
	  /// fired when a case instance is closed. </summary>
	  public static readonly HistoryEventTypes CASE_INSTANCE_CLOSE = new HistoryEventTypes("CASE_INSTANCE_CLOSE", InnerEnum.CASE_INSTANCE_CLOSE, "case-instance", "close");

	  /// <summary>
	  /// fired when a case activity instance is created. </summary>
	  public static readonly HistoryEventTypes CASE_ACTIVITY_INSTANCE_CREATE = new HistoryEventTypes("CASE_ACTIVITY_INSTANCE_CREATE", InnerEnum.CASE_ACTIVITY_INSTANCE_CREATE, "case-activity-instance", "create");
	  /// <summary>
	  /// fired when a case activity instance is updated. </summary>
	  public static readonly HistoryEventTypes CASE_ACTIVITY_INSTANCE_UPDATE = new HistoryEventTypes("CASE_ACTIVITY_INSTANCE_UPDATE", InnerEnum.CASE_ACTIVITY_INSTANCE_UPDATE, "case-activity-instance", "update");
	  /// <summary>
	  /// fired when a case instance is ended. </summary>
	  public static readonly HistoryEventTypes CASE_ACTIVITY_INSTANCE_END = new HistoryEventTypes("CASE_ACTIVITY_INSTANCE_END", InnerEnum.CASE_ACTIVITY_INSTANCE_END, "case-activity_instance", "end");

	  /// <summary>
	  /// fired when a job is created.
	  /// 
	  /// @since 7.3
	  /// </summary>
	  public static readonly HistoryEventTypes JOB_CREATE = new HistoryEventTypes("JOB_CREATE", InnerEnum.JOB_CREATE, "job", "create");

	  /// <summary>
	  /// fired when a job is failed.
	  /// 
	  /// @since 7.3
	  /// </summary>
	  public static readonly HistoryEventTypes JOB_FAIL = new HistoryEventTypes("JOB_FAIL", InnerEnum.JOB_FAIL, "job", "fail");

	  /// <summary>
	  /// fired when a job is succeeded.
	  /// 
	  /// @since 7.3
	  /// </summary>
	  public static readonly HistoryEventTypes JOB_SUCCESS = new HistoryEventTypes("JOB_SUCCESS", InnerEnum.JOB_SUCCESS, "job", "success");

	  /// <summary>
	  /// fired when a job is deleted.
	  /// 
	  /// @since 7.3
	  /// </summary>
	  public static readonly HistoryEventTypes JOB_DELETE = new HistoryEventTypes("JOB_DELETE", InnerEnum.JOB_DELETE, "job", "delete");

	  /// <summary>
	  /// fired when a decision is evaluated.
	  /// 
	  /// @since 7.4
	  /// </summary>
	  public static readonly HistoryEventTypes DMN_DECISION_EVALUATE = new HistoryEventTypes("DMN_DECISION_EVALUATE", InnerEnum.DMN_DECISION_EVALUATE, "decision", "evaluate");

	  /// <summary>
	  /// fired when a batch was started.
	  /// 
	  /// @since 7.5
	  /// </summary>
	  public static readonly HistoryEventTypes BATCH_START = new HistoryEventTypes("BATCH_START", InnerEnum.BATCH_START, "batch", "start");

	  /// <summary>
	  /// fired when a batch was completed.
	  /// 
	  /// @since 7.5
	  /// </summary>
	  public static readonly HistoryEventTypes BATCH_END = new HistoryEventTypes("BATCH_END", InnerEnum.BATCH_END, "batch", "end");

	  /// <summary>
	  /// fired when an identity link is added
	  /// 
	  /// @since 7.5
	  /// </summary>
	  public static readonly HistoryEventTypes IDENTITY_LINK_ADD = new HistoryEventTypes("IDENTITY_LINK_ADD", InnerEnum.IDENTITY_LINK_ADD, "identity-link-add", "add-identity-link");

	  /// <summary>
	  /// fired when an identity link is removed
	  /// 
	  /// @since 7.5
	  /// </summary>
	  public static readonly HistoryEventTypes IDENTITY_LINK_DELETE = new HistoryEventTypes("IDENTITY_LINK_DELETE", InnerEnum.IDENTITY_LINK_DELETE, "identity-link-delete", "delete-identity-link");

	  /// <summary>
	  /// fired when an external task is created.
	  /// 
	  /// @since 7.7
	  /// </summary>
	  public static readonly HistoryEventTypes EXTERNAL_TASK_CREATE = new HistoryEventTypes("EXTERNAL_TASK_CREATE", InnerEnum.EXTERNAL_TASK_CREATE, "external-task", "create");

	  /// <summary>
	  /// fired when an external task has failed.
	  /// 
	  /// @since 7.7
	  /// </summary>
	  public static readonly HistoryEventTypes EXTERNAL_TASK_FAIL = new HistoryEventTypes("EXTERNAL_TASK_FAIL", InnerEnum.EXTERNAL_TASK_FAIL, "external-task", "fail");

	  /// <summary>
	  /// fired when an external task has succeeded.
	  /// 
	  /// @since 7.7
	  /// </summary>
	  public static readonly HistoryEventTypes EXTERNAL_TASK_SUCCESS = new HistoryEventTypes("EXTERNAL_TASK_SUCCESS", InnerEnum.EXTERNAL_TASK_SUCCESS, "external-task", "success");

	  /// <summary>
	  /// fired when an external task is deleted.
	  /// 
	  /// @since 7.7
	  /// </summary>
	  public static readonly HistoryEventTypes EXTERNAL_TASK_DELETE = new HistoryEventTypes("EXTERNAL_TASK_DELETE", InnerEnum.EXTERNAL_TASK_DELETE, "external-task", "delete");


	  /// <summary>
	  /// fired when used operation log is created.
	  /// 
	  /// @since 7.10, 7.9.1, 7.8.7
	  /// </summary>
	  public static readonly HistoryEventTypes USER_OPERATION_LOG = new HistoryEventTypes("USER_OPERATION_LOG", InnerEnum.USER_OPERATION_LOG, "user-operation-log", "create");

	  private static readonly IList<HistoryEventTypes> valueList = new List<HistoryEventTypes>();

	  static HistoryEventTypes()
	  {
		  valueList.Add(PROCESS_INSTANCE_START);
		  valueList.Add(PROCESS_INSTANCE_UPDATE);
		  valueList.Add(PROCESS_INSTANCE_MIGRATE);
		  valueList.Add(PROCESS_INSTANCE_END);
		  valueList.Add(ACTIVITY_INSTANCE_START);
		  valueList.Add(ACTIVITY_INSTANCE_UPDATE);
		  valueList.Add(ACTIVITY_INSTANCE_MIGRATE);
		  valueList.Add(ACTIVITY_INSTANCE_END);
		  valueList.Add(TASK_INSTANCE_CREATE);
		  valueList.Add(TASK_INSTANCE_UPDATE);
		  valueList.Add(TASK_INSTANCE_MIGRATE);
		  valueList.Add(TASK_INSTANCE_COMPLETE);
		  valueList.Add(TASK_INSTANCE_DELETE);
		  valueList.Add(VARIABLE_INSTANCE_CREATE);
		  valueList.Add(VARIABLE_INSTANCE_UPDATE);
		  valueList.Add(VARIABLE_INSTANCE_MIGRATE);
		  valueList.Add(VARIABLE_INSTANCE_UPDATE_DETAIL);
		  valueList.Add(VARIABLE_INSTANCE_DELETE);
		  valueList.Add(FORM_PROPERTY_UPDATE);
		  valueList.Add(INCIDENT_CREATE);
		  valueList.Add(INCIDENT_MIGRATE);
		  valueList.Add(INCIDENT_DELETE);
		  valueList.Add(INCIDENT_RESOLVE);
		  valueList.Add(CASE_INSTANCE_CREATE);
		  valueList.Add(CASE_INSTANCE_UPDATE);
		  valueList.Add(CASE_INSTANCE_CLOSE);
		  valueList.Add(CASE_ACTIVITY_INSTANCE_CREATE);
		  valueList.Add(CASE_ACTIVITY_INSTANCE_UPDATE);
		  valueList.Add(CASE_ACTIVITY_INSTANCE_END);
		  valueList.Add(JOB_CREATE);
		  valueList.Add(JOB_FAIL);
		  valueList.Add(JOB_SUCCESS);
		  valueList.Add(JOB_DELETE);
		  valueList.Add(DMN_DECISION_EVALUATE);
		  valueList.Add(BATCH_START);
		  valueList.Add(BATCH_END);
		  valueList.Add(IDENTITY_LINK_ADD);
		  valueList.Add(IDENTITY_LINK_DELETE);
		  valueList.Add(EXTERNAL_TASK_CREATE);
		  valueList.Add(EXTERNAL_TASK_FAIL);
		  valueList.Add(EXTERNAL_TASK_SUCCESS);
		  valueList.Add(EXTERNAL_TASK_DELETE);
		  valueList.Add(USER_OPERATION_LOG);
	  }

	  public enum InnerEnum
	  {
		  PROCESS_INSTANCE_START,
		  PROCESS_INSTANCE_UPDATE,
		  PROCESS_INSTANCE_MIGRATE,
		  PROCESS_INSTANCE_END,
		  ACTIVITY_INSTANCE_START,
		  ACTIVITY_INSTANCE_UPDATE,
		  ACTIVITY_INSTANCE_MIGRATE,
		  ACTIVITY_INSTANCE_END,
		  TASK_INSTANCE_CREATE,
		  TASK_INSTANCE_UPDATE,
		  TASK_INSTANCE_MIGRATE,
		  TASK_INSTANCE_COMPLETE,
		  TASK_INSTANCE_DELETE,
		  VARIABLE_INSTANCE_CREATE,
		  VARIABLE_INSTANCE_UPDATE,
		  VARIABLE_INSTANCE_MIGRATE,
		  VARIABLE_INSTANCE_UPDATE_DETAIL,
		  VARIABLE_INSTANCE_DELETE,
		  FORM_PROPERTY_UPDATE,
		  INCIDENT_CREATE,
		  INCIDENT_MIGRATE,
		  INCIDENT_DELETE,
		  INCIDENT_RESOLVE,
		  CASE_INSTANCE_CREATE,
		  CASE_INSTANCE_UPDATE,
		  CASE_INSTANCE_CLOSE,
		  CASE_ACTIVITY_INSTANCE_CREATE,
		  CASE_ACTIVITY_INSTANCE_UPDATE,
		  CASE_ACTIVITY_INSTANCE_END,
		  JOB_CREATE,
		  JOB_FAIL,
		  JOB_SUCCESS,
		  JOB_DELETE,
		  DMN_DECISION_EVALUATE,
		  BATCH_START,
		  BATCH_END,
		  IDENTITY_LINK_ADD,
		  IDENTITY_LINK_DELETE,
		  EXTERNAL_TASK_CREATE,
		  EXTERNAL_TASK_FAIL,
		  EXTERNAL_TASK_SUCCESS,
		  EXTERNAL_TASK_DELETE,
		  USER_OPERATION_LOG
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  private HistoryEventTypes(string name, InnerEnum innerEnum, string entityType, string eventName)
	  {
		this.entityType = entityType;
		this.eventName = eventName;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  protected internal string entityType;
	  protected internal string eventName;

	  public string EntityType
	  {
		  get
		  {
			return entityType;
		  }
	  }

	  public string EventName
	  {
		  get
		  {
			return eventName;
		  }
	  }


		public static IList<HistoryEventTypes> values()
		{
			return valueList;
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static HistoryEventTypes valueOf(string name)
		{
			foreach (HistoryEventTypes enumInstance in HistoryEventTypes.valueList)
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name);
		}
	}

}