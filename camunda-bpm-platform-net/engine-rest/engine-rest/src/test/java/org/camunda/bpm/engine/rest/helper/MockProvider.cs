using System;
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
namespace org.camunda.bpm.engine.rest.helper
{
	using ProcessApplicationInfo = org.camunda.bpm.application.ProcessApplicationInfo;
	using DmnDecisionResult = org.camunda.bpm.dmn.engine.DmnDecisionResult;
	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using BatchStatistics = org.camunda.bpm.engine.batch.BatchStatistics;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using FilterQuery = org.camunda.bpm.engine.filter.FilterQuery;
	using FormField = org.camunda.bpm.engine.form.FormField;
	using FormProperty = org.camunda.bpm.engine.form.FormProperty;
	using FormType = org.camunda.bpm.engine.form.FormType;
	using StartFormData = org.camunda.bpm.engine.form.StartFormData;
	using TaskFormData = org.camunda.bpm.engine.form.TaskFormData;
	using DurationReportResult = org.camunda.bpm.engine.history.DurationReportResult;
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricActivityStatistics = org.camunda.bpm.engine.history.HistoricActivityStatistics;
	using HistoricCaseActivityInstance = org.camunda.bpm.engine.history.HistoricCaseActivityInstance;
	using HistoricCaseActivityStatistics = org.camunda.bpm.engine.history.HistoricCaseActivityStatistics;
	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using HistoricDecisionInputInstance = org.camunda.bpm.engine.history.HistoricDecisionInputInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionInstanceStatistics = org.camunda.bpm.engine.history.HistoricDecisionInstanceStatistics;
	using HistoricDecisionOutputInstance = org.camunda.bpm.engine.history.HistoricDecisionOutputInstance;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using HistoricFormField = org.camunda.bpm.engine.history.HistoricFormField;
	using HistoricIdentityLinkLog = org.camunda.bpm.engine.history.HistoricIdentityLinkLog;
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricTaskInstanceReportResult = org.camunda.bpm.engine.history.HistoricTaskInstanceReportResult;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using Group = org.camunda.bpm.engine.identity.Group;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using User = org.camunda.bpm.engine.identity.User;
	using TaskQueryImpl = org.camunda.bpm.engine.impl.TaskQueryImpl;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using MetricIntervalEntity = org.camunda.bpm.engine.impl.persistence.entity.MetricIntervalEntity;
	using ResourceEntity = org.camunda.bpm.engine.impl.persistence.entity.ResourceEntity;
	using ActivityStatistics = org.camunda.bpm.engine.management.ActivityStatistics;
	using IncidentStatistics = org.camunda.bpm.engine.management.IncidentStatistics;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using MetricIntervalValue = org.camunda.bpm.engine.management.MetricIntervalValue;
	using MetricsQuery = org.camunda.bpm.engine.management.MetricsQuery;
	using ProcessDefinitionStatistics = org.camunda.bpm.engine.management.ProcessDefinitionStatistics;
	using PeriodUnit = org.camunda.bpm.engine.query.PeriodUnit;
	using Query = org.camunda.bpm.engine.query.Query;
	using org.camunda.bpm.engine.repository;
	using TaskQueryDto = org.camunda.bpm.engine.rest.dto.task.TaskQueryDto;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using MessageCorrelationResult = org.camunda.bpm.engine.runtime.MessageCorrelationResult;
	using MessageCorrelationResultType = org.camunda.bpm.engine.runtime.MessageCorrelationResultType;
	using MessageCorrelationResultWithVariables = org.camunda.bpm.engine.runtime.MessageCorrelationResultWithVariables;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceWithVariables = org.camunda.bpm.engine.runtime.ProcessInstanceWithVariables;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Attachment = org.camunda.bpm.engine.task.Attachment;
	using Comment = org.camunda.bpm.engine.task.Comment;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskCountByCandidateGroupResult = org.camunda.bpm.engine.task.TaskCountByCandidateGroupResult;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ObjectValueImpl = org.camunda.bpm.engine.variable.impl.value.ObjectValueImpl;
	using BytesValue = org.camunda.bpm.engine.variable.value.BytesValue;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using StringValue = org.camunda.bpm.engine.variable.value.StringValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.util.DateTimeUtils.withTimezone;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doThrow;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	/// <summary>
	/// Provides mocks for the basic engine entities, such as
	/// <seealso cref="ProcessDefinition"/>, <seealso cref="User"/>, etc., that are reused across the
	/// various kinds of tests.
	/// 
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public abstract class MockProvider
	{

	  public const string FORMAT_APPLICATION_JSON = "application/json";

	  // general non existing Id
	  public const string NON_EXISTING_ID = "nonExistingId";

	  // tenant ids
	  public const string EXAMPLE_TENANT_ID = "aTenantId";
	  public const string ANOTHER_EXAMPLE_TENANT_ID = "anotherTenantId";
	  public static readonly string EXAMPLE_TENANT_ID_LIST = EXAMPLE_TENANT_ID + "," + ANOTHER_EXAMPLE_TENANT_ID;

	  public const string EXAMPLE_TENANT_NAME = "aTenantName";

	  // case activity ids
	  public const string EXAMPLE_CASE_ACTIVITY_ID = "aCaseActivityId";
	  public const string ANOTHER_EXAMPLE_CASE_ACTIVITY_ID = "anotherCaseActivityId";
	  public static readonly string EXAMPLE_CASE_ACTIVITY_ID_LIST = EXAMPLE_CASE_ACTIVITY_ID + "," + ANOTHER_EXAMPLE_CASE_ACTIVITY_ID;

	  // version tag
	  public const string EXAMPLE_VERSION_TAG = "aVersionTag";
	  public const string ANOTHER_EXAMPLE_VERSION_TAG = "anotherVersionTag";

	  // engine
	  public const string EXAMPLE_PROCESS_ENGINE_NAME = "default";
	  public const string ANOTHER_EXAMPLE_PROCESS_ENGINE_NAME = "anotherEngineName";
	  public const string NON_EXISTING_PROCESS_ENGINE_NAME = "aNonExistingEngineName";

	  // task properties
	  public const string EXAMPLE_TASK_ID = "anId";
	  public const string EXAMPLE_TASK_NAME = "aName";
	  public const string EXAMPLE_TASK_ASSIGNEE_NAME = "anAssignee";
	  public static readonly string EXAMPLE_TASK_CREATE_TIME = withTimezone("2013-01-23T13:42:42");
	  public static readonly string EXAMPLE_TASK_DUE_DATE = withTimezone("2013-01-23T13:42:43");
	  public static readonly string EXAMPLE_FOLLOW_UP_DATE = withTimezone("2013-01-23T13:42:44");
	  public const DelegationState EXAMPLE_TASK_DELEGATION_STATE = DelegationState.RESOLVED;
	  public const string EXAMPLE_TASK_DESCRIPTION = "aDescription";
	  public const string EXAMPLE_TASK_EXECUTION_ID = "anExecution";
	  public const string EXAMPLE_TASK_OWNER = "anOwner";
	  public const string EXAMPLE_TASK_PARENT_TASK_ID = "aParentId";
	  public const int EXAMPLE_TASK_PRIORITY = 42;
	  public const string EXAMPLE_TASK_DEFINITION_KEY = "aTaskDefinitionKey";
	  public const bool EXAMPLE_TASK_SUSPENSION_STATE = false;

	  // task comment
	  public const string EXAMPLE_TASK_COMMENT_ID = "aTaskCommentId";
	  public const string EXAMPLE_TASK_COMMENT_FULL_MESSAGE = "aTaskCommentFullMessage";
	  public static readonly string EXAMPLE_TASK_COMMENT_TIME = withTimezone("2014-04-24T14:10:44");
	  public const string EXAMPLE_TASK_COMMENT_ROOT_PROCESS_INSTANCE_ID = "aRootProcInstId";

	  // task attachment
	  public const string EXAMPLE_TASK_ATTACHMENT_ID = "aTaskAttachmentId";
	  public const string EXAMPLE_TASK_ATTACHMENT_NAME = "aTaskAttachmentName";
	  public const string EXAMPLE_TASK_ATTACHMENT_DESCRIPTION = "aTaskAttachmentDescription";
	  public const string EXAMPLE_TASK_ATTACHMENT_TYPE = "aTaskAttachmentType";
	  public const string EXAMPLE_TASK_ATTACHMENT_URL = "aTaskAttachmentUrl";
	  public static readonly string EXAMPLE_TASK_ATTACHMENT_CREATE_DATE = withTimezone("2018-07-19T15:02:36");
	  public static readonly string EXAMPLE_TASK_ATTACHMENT_REMOVAL_DATE = withTimezone("2018-10-17T13:35:07");
	  public const string EXAMPLE_TASK_ATTACHMENT_ROOT_PROCESS_INSTANCE_ID = "aRootProcInstId";

	  // task count by candidate group

	  public const int EXAMPLE_TASK_COUNT_BY_CANDIDATE_GROUP = 2;

	  // form data
	  public const string EXAMPLE_FORM_KEY = "aFormKey";
	  public const string EXAMPLE_DEPLOYMENT_ID = "aDeploymentId";
	  public const string EXAMPLE_RE_DEPLOYMENT_ID = "aReDeploymentId";

	  // form property data
	  public const string EXAMPLE_FORM_PROPERTY_ID = "aFormPropertyId";
	  public const string EXAMPLE_FORM_PROPERTY_NAME = "aFormName";
	  public const string EXAMPLE_FORM_PROPERTY_TYPE_NAME = "aFormPropertyTypeName";
	  public const string EXAMPLE_FORM_PROPERTY_VALUE = "aValue";
	  public const bool EXAMPLE_FORM_PROPERTY_READABLE = true;
	  public const bool EXAMPLE_FORM_PROPERTY_WRITABLE = true;
	  public const bool EXAMPLE_FORM_PROPERTY_REQUIRED = true;

	  // process instance
	  public const string EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY = "aKey";
	  public const string EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY_LIKE = "aKeyLike";
	  public const string EXAMPLE_PROCESS_INSTANCE_ID = "aProcInstId";
	  public const string EXAMPLE_ROOT_HISTORIC_PROCESS_INSTANCE_ID = "aRootProcInstId";
	  public const string ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID = "anotherId";
	  public const bool EXAMPLE_PROCESS_INSTANCE_IS_SUSPENDED = false;
	  public const bool EXAMPLE_PROCESS_INSTANCE_IS_ENDED = false;
	  public static readonly string EXAMPLE_PROCESS_INSTANCE_ID_LIST = EXAMPLE_PROCESS_INSTANCE_ID + "," + ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID;
	  public static readonly string EXAMPLE_PROCESS_INSTANCE_ID_LIST_WITH_DUP = EXAMPLE_PROCESS_INSTANCE_ID + "," + ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID + "," + EXAMPLE_PROCESS_INSTANCE_ID;
	  public const string EXAMPLE_NON_EXISTENT_PROCESS_INSTANCE_ID = "aNonExistentProcInstId";
	  public static readonly string EXAMPLE_PROCESS_INSTANCE_ID_LIST_WITH_NONEXISTENT_ID = EXAMPLE_PROCESS_INSTANCE_ID + "," + EXAMPLE_NON_EXISTENT_PROCESS_INSTANCE_ID;

	  // variable instance
	  public const string EXAMPLE_VARIABLE_INSTANCE_ID = "aVariableInstanceId";

	  public const string SERIALIZABLE_VARIABLE_INSTANCE_ID = "serializableVariableInstanceId";
	  public const string SPIN_VARIABLE_INSTANCE_ID = "spinVariableInstanceId";

	  public const string EXAMPLE_VARIABLE_INSTANCE_NAME = "aVariableInstanceName";
	  public const string EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME = "aDeserializedVariableInstanceName";

	  public static readonly StringValue EXAMPLE_PRIMITIVE_VARIABLE_VALUE = Variables.stringValue("aVariableInstanceValue");
	  public const string EXAMPLE_VARIABLE_INSTANCE_PROC_DEF_KEY = "aVariableInstanceProcDefKey";
	  public const string EXAMPLE_VARIABLE_INSTANCE_PROC_DEF_ID = "aVariableInstanceProcDefId";
	  public const string EXAMPLE_VARIABLE_INSTANCE_PROC_INST_ID = "aVariableInstanceProcInstId";
	  public const string EXAMPLE_VARIABLE_INSTANCE_EXECUTION_ID = "aVariableInstanceExecutionId";
	  public const string EXAMPLE_VARIABLE_INSTANCE_CASE_INST_ID = "aVariableInstanceCaseInstId";
	  public const string EXAMPLE_VARIABLE_INSTANCE_CASE_EXECUTION_ID = "aVariableInstanceCaseExecutionId";
	  public const string EXAMPLE_VARIABLE_INSTANCE_TASK_ID = "aVariableInstanceTaskId";
	  public const string EXAMPLE_VARIABLE_INSTANCE_ACTIVITY_INSTANCE_ID = "aVariableInstanceVariableInstanceId";
	  public const string EXAMPLE_VARIABLE_INSTANCE_ERROR_MESSAGE = "aVariableInstanceErrorMessage";
	  public const string EXAMPLE_VARIABLE_INSTANCE_CASE_DEF_KEY = "aVariableInstanceCaseDefKey";
	  public const string EXAMPLE_VARIABLE_INSTANCE_CASE_DEF_ID = "aVariableInstanceCaseDefId";
	  public static readonly string EXAMPLE_HISTORIC_VARIABLE_INSTANCE_CREATE_TIME = withTimezone("2013-04-23T13:42:43");
	  public static readonly string EXAMPLE_HISTORIC_VARIABLE_INSTANCE_REMOVAL_TIME = withTimezone("2018-04-23T13:42:43");
	  public const string EXAMPLE_HISTORIC_VARIABLE_INSTANCE_ROOT_PROC_INST_ID = "aRootProcInstId";

	  public const string EXAMPLE_VARIABLE_INSTANCE_SERIALIZED_VALUE = "aSerializedValue";
	  public static readonly sbyte[] EXAMPLE_VARIABLE_INSTANCE_BYTE = "aSerializedValue".Bytes;
	  public const string EXAMPLE_VARIABLE_INSTANCE_DESERIALIZED_VALUE = "aDeserializedValue";

	  public const string EXAMPLE_SPIN_DATA_FORMAT = "aDataFormatId";
	  public const string EXAMPLE_SPIN_ROOT_TYPE = "path.to.a.RootType";


	  // execution
	  public const string EXAMPLE_EXECUTION_ID = "anExecutionId";
	  public const bool EXAMPLE_EXECUTION_IS_ENDED = false;

	  // event subscription
	  public const string EXAMPLE_EVENT_SUBSCRIPTION_ID = "anEventSubscriptionId";
	  public const string EXAMPLE_EVENT_SUBSCRIPTION_TYPE = "message";
	  public const string EXAMPLE_EVENT_SUBSCRIPTION_NAME = "anEvent";
	  public static readonly string EXAMPLE_EVENT_SUBSCRIPTION_CREATION_DATE = withTimezone("2013-01-23T13:59:43");

	  // process definition
	  public const string EXAMPLE_PROCESS_DEFINITION_ID = "aProcDefId";
	  public const string NON_EXISTING_PROCESS_DEFINITION_ID = "aNonExistingProcDefId";
	  public const string EXAMPLE_PROCESS_DEFINITION_NAME = "aName";
	  public const string EXAMPLE_PROCESS_DEFINITION_NAME_LIKE = "aNameLike";
	  public const string EXAMPLE_PROCESS_DEFINITION_KEY = "aKey";
	  public const string ANOTHER_EXAMPLE_PROCESS_DEFINITION_KEY = "anotherProcessDefinitionKey";
	  public static readonly string EXAMPLE_KEY_LIST = EXAMPLE_PROCESS_DEFINITION_KEY + "," + ANOTHER_EXAMPLE_PROCESS_DEFINITION_KEY;

	  public const string NON_EXISTING_PROCESS_DEFINITION_KEY = "aNonExistingKey";
	  public const string EXAMPLE_PROCESS_DEFINITION_CATEGORY = "aCategory";
	  public const string EXAMPLE_PROCESS_DEFINITION_DESCRIPTION = "aDescription";
	  public const int EXAMPLE_PROCESS_DEFINITION_VERSION = 42;
	  public const string EXAMPLE_PROCESS_DEFINITION_RESOURCE_NAME = "aResourceName";
	  public const string EXAMPLE_PROCESS_DEFINITION_DIAGRAM_RESOURCE_NAME = "aResourceName.png";
	  public const bool EXAMPLE_PROCESS_DEFINITION_IS_SUSPENDED = true;
	  public const bool EXAMPLE_PROCESS_DEFINITION_IS_STARTABLE = true;

	  public const string ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID = "aProcessDefinitionId:2";
	  public static readonly string EXAMPLE_PROCESS_DEFINTION_ID_LIST = EXAMPLE_PROCESS_DEFINITION_ID + "," + ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID;

	  public const string EXAMPLE_ACTIVITY_ID = "anActivity";
	  public const string ANOTHER_EXAMPLE_ACTIVITY_ID = "anotherActivity";
	  public static readonly string EXAMPLE_ACTIVITY_ID_LIST = EXAMPLE_ACTIVITY_ID + "," + ANOTHER_EXAMPLE_ACTIVITY_ID;
	  public const string NON_EXISTING_ACTIVITY_ID = "aNonExistingActivityId";
	  public const string EXAMPLE_ACTIVITY_INSTANCE_ID = "anActivityInstanceId";
	  public const string EXAMPLE_ACTIVITY_NAME = "anActivityName";
	  public const string EXAMPLE_ACTIVITY_TYPE = "anActivityType";
	  public static readonly string EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION = withTimezone("2013-04-23T13:42:43");

	  // deployment
	  public const string NON_EXISTING_DEPLOYMENT_ID = "aNonExistingDeploymentId";
	  public const string EXAMPLE_DEPLOYMENT_NAME = "aName";
	  public const string EXAMPLE_DEPLOYMENT_NAME_LIKE = "aNameLike";
	  public const string EXAMPLE_DEPLOYMENT_SOURCE = "aDeploymentSource";
	  public static readonly string EXAMPLE_DEPLOYMENT_TIME = withTimezone("2013-01-23T13:59:43");
	  public static readonly string EXAMPLE_DEPLOYMENT_TIME_BEFORE = withTimezone("2013-01-03T13:59:43");
	  public static readonly string EXAMPLE_DEPLOYMENT_TIME_AFTER = withTimezone("2013-03-23T13:59:43");
	  public static readonly string NON_EXISTING_DEPLOYMENT_TIME = withTimezone("2013-04-23T13:42:43");

	  // deployment resources
	  public const string EXAMPLE_DEPLOYMENT_RESOURCE_ID = "aDeploymentResourceId";
	  public const string NON_EXISTING_DEPLOYMENT_RESOURCE_ID = "aNonExistingDeploymentResourceId";
	  public const string EXAMPLE_DEPLOYMENT_RESOURCE_NAME = "aDeploymentResourceName";

	  public const string EXAMPLE_DEPLOYMENT_SVG_RESOURCE_ID = "aDeploymentSvgResourceId";
	  public const string EXAMPLE_DEPLOYMENT_SVG_RESOURCE_NAME = "a-svg-resource.svg";

	  public const string EXAMPLE_DEPLOYMENT_PNG_RESOURCE_ID = "aDeploymentPngResourceId";
	  public const string EXAMPLE_DEPLOYMENT_PNG_RESOURCE_NAME = "an-image-resource.png";

	  public const string EXAMPLE_DEPLOYMENT_JPG_RESOURCE_ID = "aDeploymentJpgResourceId";
	  public const string EXAMPLE_DEPLOYMENT_JPG_RESOURCE_NAME = "an-image-resource.jpg";

	  public const string EXAMPLE_DEPLOYMENT_JPEG_RESOURCE_ID = "aDeploymentJpegResourceId";
	  public const string EXAMPLE_DEPLOYMENT_JPEG_RESOURCE_NAME = "an-image-resource.jpeg";

	  public const string EXAMPLE_DEPLOYMENT_JPE_RESOURCE_ID = "aDeploymentJpeResourceId";
	  public const string EXAMPLE_DEPLOYMENT_JPE_RESOURCE_NAME = "an-image-resource.jpe";

	  public const string EXAMPLE_DEPLOYMENT_GIF_RESOURCE_ID = "aDeploymentGifResourceId";
	  public const string EXAMPLE_DEPLOYMENT_GIF_RESOURCE_NAME = "an-image-resource.gif";

	  public const string EXAMPLE_DEPLOYMENT_TIF_RESOURCE_ID = "aDeploymentTifResourceId";
	  public const string EXAMPLE_DEPLOYMENT_TIF_RESOURCE_NAME = "an-image-resource.tif";

	  public const string EXAMPLE_DEPLOYMENT_TIFF_RESOURCE_ID = "aDeploymentTiffResourceId";
	  public const string EXAMPLE_DEPLOYMENT_TIFF_RESOURCE_NAME = "an-image-resource.tiff";

	  public const string EXAMPLE_DEPLOYMENT_BPMN_RESOURCE_ID = "aDeploymentBpmnResourceId";
	  public const string EXAMPLE_DEPLOYMENT_BPMN_RESOURCE_NAME = "a-bpmn-resource.bpmn";

	  public const string EXAMPLE_DEPLOYMENT_BPMN_XML_RESOURCE_ID = "aDeploymentBpmnXmlResourceId";
	  public const string EXAMPLE_DEPLOYMENT_BPMN_XML_RESOURCE_NAME = "a-bpmn-resource.bpmn20.xml";

	  public const string EXAMPLE_DEPLOYMENT_CMMN_RESOURCE_ID = "aDeploymentCmmnResourceId";
	  public const string EXAMPLE_DEPLOYMENT_CMMN_RESOURCE_NAME = "a-cmmn-resource.cmmn";

	  public const string EXAMPLE_DEPLOYMENT_CMMN_XML_RESOURCE_ID = "aDeploymentCmmnXmlResourceId";
	  public const string EXAMPLE_DEPLOYMENT_CMMN_XML_RESOURCE_NAME = "a-cmmn-resource.cmmn10.xml";

	  public const string EXAMPLE_DEPLOYMENT_DMN_RESOURCE_ID = "aDeploymentDmnResourceId";
	  public const string EXAMPLE_DEPLOYMENT_DMN_RESOURCE_NAME = "a-dmn-resource.dmn";

	  public const string EXAMPLE_DEPLOYMENT_DMN_XML_RESOURCE_ID = "aDeploymentDmnXmlResourceId";
	  public const string EXAMPLE_DEPLOYMENT_DMN_XML_RESOURCE_NAME = "a-dmn-resource.dmn11.xml";

	  public const string EXAMPLE_DEPLOYMENT_XML_RESOURCE_ID = "aDeploymentXmlResourceId";
	  public const string EXAMPLE_DEPLOYMENT_XML_RESOURCE_NAME = "a-xml-resource.xml";

	  public const string EXAMPLE_DEPLOYMENT_JSON_RESOURCE_ID = "aDeploymentJsonResourceId";
	  public const string EXAMPLE_DEPLOYMENT_JSON_RESOURCE_NAME = "a-json-resource.json";

	  public const string EXAMPLE_DEPLOYMENT_GROOVY_RESOURCE_ID = "aDeploymentGroovyResourceId";
	  public const string EXAMPLE_DEPLOYMENT_GROOVY_RESOURCE_NAME = "a-groovy-resource.groovy";

	  public const string EXAMPLE_DEPLOYMENT_JAVA_RESOURCE_ID = "aDeploymentGroovyResourceId";
	  public const string EXAMPLE_DEPLOYMENT_JAVA_RESOURCE_NAME = "a-java-resource.java";

	  public const string EXAMPLE_DEPLOYMENT_JS_RESOURCE_ID = "aDeploymentJsResourceId";
	  public const string EXAMPLE_DEPLOYMENT_JS_RESOURCE_NAME = "a-js-resource.js";

	  public const string EXAMPLE_DEPLOYMENT_PHP_RESOURCE_ID = "aDeploymentPhpResourceId";
	  public const string EXAMPLE_DEPLOYMENT_PHP_RESOURCE_NAME = "a-php-resource.php";

	  public const string EXAMPLE_DEPLOYMENT_PYTHON_RESOURCE_ID = "aDeploymentPythonResourceId";
	  public const string EXAMPLE_DEPLOYMENT_PYTHON_RESOURCE_NAME = "a-python-resource.py";

	  public const string EXAMPLE_DEPLOYMENT_RUBY_RESOURCE_ID = "aDeploymentRubyResourceId";
	  public const string EXAMPLE_DEPLOYMENT_RUBY_RESOURCE_NAME = "a-ruby-resource.rb";

	  public const string EXAMPLE_DEPLOYMENT_HTML_RESOURCE_ID = "aDeploymentHtmlResourceId";
	  public const string EXAMPLE_DEPLOYMENT_HTML_RESOURCE_NAME = "a-html-resource.html";

	  public const string EXAMPLE_DEPLOYMENT_TXT_RESOURCE_ID = "aDeploymentTxtResourceId";
	  public const string EXAMPLE_DEPLOYMENT_TXT_RESOURCE_NAME = "a-txt-resource.txt";

	  public const string EXAMPLE_DEPLOYMENT_RESOURCE_FILENAME_ID = "aDeploymentResourceFilenameId";
	  public const string EXAMPLE_DEPLOYMENT_RESOURCE_FILENAME_PATH = "my/path/to/my/bpmn/";
	  public const string EXAMPLE_DEPLOYMENT_RESOURCE_FILENAME_PATH_BACKSLASH = "my\\path\\to\\my\\bpmn\\";
	  public const string EXAMPLE_DEPLOYMENT_RESOURCE_FILENAME_NAME = "process.bpmn";

	  // statistics
	  public const int EXAMPLE_FAILED_JOBS = 42;
	  public const int EXAMPLE_INSTANCES = 123;

	  public const long EXAMPLE_INSTANCES_LONG = 123;
	  public const long EXAMPLE_FINISHED_LONG = 124;
	  public const long EXAMPLE_CANCELED_LONG = 125;
	  public const long EXAMPLE_COMPLETE_SCOPE_LONG = 126;

	  public const long ANOTHER_EXAMPLE_INSTANCES_LONG = 127;
	  public const long ANOTHER_EXAMPLE_FINISHED_LONG = 128;
	  public const long ANOTHER_EXAMPLE_CANCELED_LONG = 129;
	  public const long ANOTHER_EXAMPLE_COMPLETE_SCOPE_LONG = 130;

	  public const long EXAMPLE_AVAILABLE_LONG = 123;
	  public const long EXAMPLE_ACTIVE_LONG = 124;
	  public const long EXAMPLE_COMPLETED_LONG = 125;
	  public const long EXAMPLE_DISABLED_LONG = 126;
	  public const long EXAMPLE_ENABLED_LONG = 127;
	  public const long EXAMPLE_TERMINATED_LONG = 128;

	  public const long ANOTHER_EXAMPLE_AVAILABLE_LONG = 129;
	  public const long ANOTHER_EXAMPLE_ACTIVE_LONG = 130;
	  public const long ANOTHER_EXAMPLE_COMPLETED_LONG = 131;
	  public const long ANOTHER_EXAMPLE_DISABLED_LONG = 132;
	  public const long ANOTHER_EXAMPLE_ENABLED_LONG = 133;
	  public const long ANOTHER_EXAMPLE_TERMINATED_LONG = 134;

	  public const int ANOTHER_EXAMPLE_FAILED_JOBS = 43;
	  public const int ANOTHER_EXAMPLE_INSTANCES = 124;

	  public const string ANOTHER_EXAMPLE_INCIDENT_TYPE = "anotherIncidentType";
	  public const int ANOTHER_EXAMPLE_INCIDENT_COUNT = 2;

	  // user & groups
	  public const string EXAMPLE_GROUP_ID = "groupId1";
	  public const string EXAMPLE_GROUP_ID2 = "groupId2";
	  public const string EXAMPLE_GROUP_NAME = "group1";
	  public const string EXAMPLE_GROUP_TYPE = "organizational-unit";
	  public const string EXAMPLE_GROUP_NAME_UPDATE = "group1Update";

	  public const string EXAMPLE_USER_ID = "userId";
	  public const string EXAMPLE_USER_ID2 = "userId2";
	  public const string EXAMPLE_USER_FIRST_NAME = "firstName";
	  public const string EXAMPLE_USER_LAST_NAME = "lastName";
	  public const string EXAMPLE_USER_EMAIL = "test@example.org";
	  public const string EXAMPLE_USER_PASSWORD = "s3cret";

	  public const string EXAMPLE_USER_FIRST_NAME_UPDATE = "firstNameUpdate";
	  public const string EXAMPLE_USER_LAST_NAME_UPDATE = "lastNameUpdate";
	  public const string EXAMPLE_USER_EMAIL_UPDATE = "testUpdate@example.org";

	  // Job Definitions
	  public const string EXAMPLE_JOB_DEFINITION_ID = "aJobDefId";
	  public const string NON_EXISTING_JOB_DEFINITION_ID = "aNonExistingJobDefId";
	  public const string EXAMPLE_JOB_TYPE = "aJobType";
	  public const string EXAMPLE_JOB_CONFIG = "aJobConfig";
	  public const bool EXAMPLE_JOB_DEFINITION_IS_SUSPENDED = true;
	  public static readonly string EXAMPLE_JOB_DEFINITION_DELAYED_EXECUTION = withTimezone("2013-04-23T13:42:43");
	  public static readonly long EXAMPLE_JOB_DEFINITION_PRIORITY = int.MaxValue + 52l;

	  // Jobs
	  public const string EXAMPLE_JOB_ACTIVITY_ID = "aJobActivityId";
	  public const string EXAMPLE_JOB_ID = "aJobId";
	  public const string NON_EXISTING_JOB_ID = "aNonExistingJobId";
	  public const int EXAMPLE_NEGATIVE_JOB_RETRIES = -3;
	  public const int EXAMPLE_JOB_RETRIES = 3;
	  public const string EXAMPLE_JOB_NO_EXCEPTION_MESSAGE = "";
	  public const string EXAMPLE_EXCEPTION_MESSAGE = "aExceptionMessage";
	  public const string EXAMPLE_EMPTY_JOB_ID = "";
	  public static readonly string EXAMPLE_DUE_DATE = withTimezone("2013-04-23T13:42:43");
	  public const bool? EXAMPLE_WITH_RETRIES_LEFT = true;
	  public const bool? EXAMPLE_EXECUTABLE = true;
	  public const bool? EXAMPLE_TIMERS = true;
	  public const bool? EXAMPLE_MESSAGES = true;
	  public const bool? EXAMPLE_WITH_EXCEPTION = true;
	  public const bool? EXAMPLE_NO_RETRIES_LEFT = true;
	  public const bool? EXAMPLE_JOB_IS_SUSPENDED = true;
	  public static readonly long EXAMPLE_JOB_PRIORITY = int.MaxValue + 42l;

	  public const string EXAMPLE_RESOURCE_TYPE_NAME = "exampleResource";
	  public const int EXAMPLE_RESOURCE_TYPE_ID = 12345678;
	  public const string EXAMPLE_RESOURCE_TYPE_ID_STRING = "12345678";
	  public const string EXAMPLE_RESOURCE_ID = "exampleResourceId";
	  public const string EXAMPLE_PERMISSION_NAME = "READ";
	  public static readonly Permission[] EXAMPLE_GRANT_PERMISSION_VALUES = new Permission[] {Permissions.NONE, Permissions.READ, Permissions.UPDATE};
	  public static readonly Permission[] EXAMPLE_REVOKE_PERMISSION_VALUES = new Permission[] {Permissions.ALL, Permissions.READ, Permissions.UPDATE};
	  public static readonly string[] EXAMPLE_PERMISSION_VALUES_STRING = new string[] {"READ", "UPDATE"};

	  public const string EXAMPLE_AUTHORIZATION_ID = "someAuthorizationId";
	  public const int EXAMPLE_AUTHORIZATION_TYPE = 0;
	  public const string EXAMPLE_AUTHORIZATION_TYPE_STRING = "0";

	  // process applications
	  public const string EXAMPLE_PROCESS_APPLICATION_NAME = "aProcessApplication";
	  public const string EXAMPLE_PROCESS_APPLICATION_CONTEXT_PATH = "http://camunda.org/someContext";

	  // Historic Process Instance
	  public const string EXAMPLE_HISTORIC_PROCESS_INSTANCE_DELETE_REASON = "aDeleteReason";
	  public const long EXAMPLE_HISTORIC_PROCESS_INSTANCE_DURATION_MILLIS = 2000l;
	  public static readonly string EXAMPLE_HISTORIC_PROCESS_INSTANCE_START_TIME = withTimezone("2013-04-23T13:42:43");
	  public static readonly string EXAMPLE_HISTORIC_PROCESS_INSTANCE_END_TIME = withTimezone("2013-04-23T13:42:43");
	  public static readonly string EXAMPLE_HISTORIC_PROCESS_INSTANCE_REMOVAL_TIME = withTimezone("2013-04-26T13:42:43");
	  public const string EXAMPLE_HISTORIC_PROCESS_INSTANCE_START_USER_ID = "aStartUserId";
	  public const string EXAMPLE_HISTORIC_PROCESS_INSTANCE_START_ACTIVITY_ID = "aStartActivityId";
	  public const string EXAMPLE_HISTORIC_PROCESS_INSTANCE_ROOT_PROCESS_INSTANCE_ID = "aRootProcessInstanceId";
	  public const string EXAMPLE_HISTORIC_PROCESS_INSTANCE_SUPER_PROCESS_INSTANCE_ID = "aSuperProcessInstanceId";
	  public const string EXAMPLE_HISTORIC_PROCESS_INSTANCE_SUPER_CASE_INSTANCE_ID = "aSuperCaseInstanceId";
	  public const string EXAMPLE_HISTORIC_PROCESS_INSTANCE_SUB_PROCESS_INSTANCE_ID = "aSubProcessInstanceId";
	  public const string EXAMPLE_HISTORIC_PROCESS_INSTANCE_CASE_INSTANCE_ID = "aCaseInstanceId";
	  public const string EXAMPLE_HISTORIC_PROCESS_INSTANCE_SUB_CASE_INSTANCE_ID = "aSubCaseInstanceId";
	  public const string EXAMPLE_HISTORIC_PROCESS_INSTANCE_STATE = "aState";

	  public static readonly string EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_AFTER = withTimezone("2013-04-23T13:42:43");
	  public static readonly string EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_BEFORE = withTimezone("2013-01-23T13:42:43");
	  public static readonly string EXAMPLE_HISTORIC_PROCESS_INSTANCE_FINISHED_AFTER = withTimezone("2013-01-23T13:42:43");
	  public static readonly string EXAMPLE_HISTORIC_PROCESS_INSTANCE_FINISHED_BEFORE = withTimezone("2013-04-23T13:42:43");

	  // historic process instance duration report
	  public const long EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_AVG = 10;
	  public const long EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MIN = 5;
	  public const long EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MAX = 15;
	  public const int EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_PERIOD = 1;

	  // Historic Case Instance
	  public const long EXAMPLE_HISTORIC_CASE_INSTANCE_DURATION_MILLIS = 2000l;
	  public static readonly string EXAMPLE_HISTORIC_CASE_INSTANCE_CREATE_TIME = withTimezone("2013-04-23T13:42:43");
	  public static readonly string EXAMPLE_HISTORIC_CASE_INSTANCE_CLOSE_TIME = withTimezone("2013-04-23T13:42:43");
	  public const string EXAMPLE_HISTORIC_CASE_INSTANCE_CREATE_USER_ID = "aCreateUserId";
	  public const string EXAMPLE_HISTORIC_CASE_INSTANCE_SUPER_CASE_INSTANCE_ID = "aSuperCaseInstanceId";
	  public const string EXAMPLE_HISTORIC_CASE_INSTANCE_SUB_CASE_INSTANCE_ID = "aSubCaseInstanceId";
	  public const string EXAMPLE_HISTORIC_CASE_INSTANCE_SUPER_PROCESS_INSTANCE_ID = "aSuperProcessInstanceId";
	  public const string EXAMPLE_HISTORIC_CASE_INSTANCE_SUB_PROCESS_INSTANCE_ID = "aSuperProcessInstanceId";

	  public static readonly string EXAMPLE_HISTORIC_CASE_INSTANCE_CREATED_AFTER = withTimezone("2013-04-23T13:42:43");
	  public static readonly string EXAMPLE_HISTORIC_CASE_INSTANCE_CREATED_BEFORE = withTimezone("2013-01-23T13:42:43");
	  public static readonly string EXAMPLE_HISTORIC_CASE_INSTANCE_CLOSED_AFTER = withTimezone("2013-01-23T13:42:43");
	  public static readonly string EXAMPLE_HISTORIC_CASE_INSTANCE_CLOSED_BEFORE = withTimezone("2013-04-23T13:42:43");

	  public const bool EXAMPLE_HISTORIC_CASE_INSTANCE_IS_ACTIVE = true;
	  public const bool EXAMPLE_HISTORIC_CASE_INSTANCE_IS_COMPLETED = true;
	  public const bool EXAMPLE_HISTORIC_CASE_INSTANCE_IS_TERMINATED = true;
	  public const bool EXAMPLE_HISTORIC_CASE_INSTANCE_IS_CLOSED = true;

	  // Historic Activity Instance
	  public const string EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_ID = "aHistoricActivityInstanceId";
	  public const string EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_PARENT_ACTIVITY_INSTANCE_ID = "aHistoricParentActivityInstanceId";
	  public const string EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_CALLED_PROCESS_INSTANCE_ID = "aHistoricCalledProcessInstanceId";
	  public const string EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_CALLED_CASE_INSTANCE_ID = "aHistoricCalledCaseInstanceId";
	  public static readonly string EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_START_TIME = withTimezone("2013-04-23T13:42:43");
	  public static readonly string EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_END_TIME = withTimezone("2013-04-23T18:42:43");
	  public static readonly string EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_REMOVAL_TIME = withTimezone("2013-04-23T13:42:43");
	  public const long EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_DURATION = 2000l;
	  public static readonly string EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_STARTED_AFTER = withTimezone("2013-04-23T13:42:43");
	  public static readonly string EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_STARTED_BEFORE = withTimezone("2013-01-23T13:42:43");
	  public static readonly string EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_FINISHED_AFTER = withTimezone("2013-01-23T13:42:43");
	  public static readonly string EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_FINISHED_BEFORE = withTimezone("2013-04-23T13:42:43");
	  public const bool EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_IS_CANCELED = true;
	  public const bool EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_IS_COMPLETE_SCOPE = true;
	  public const string EXAMPLE_HISTORIC_ACTIVITY_ROOT_PROCESS_INSTANCE_ID = "aRootProcInstId";

	  // Historic Case Activity Instance
	  public const string EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_ID = "aCaseActivityInstanceId";
	  public const string EXAMPLE_HISTORIC_ANOTHER_CASE_ACTIVITY_INSTANCE_ID = "anotherCaseActivityInstanceId";
	  public const string EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_PARENT_CASE_ACTIVITY_INSTANCE_ID = "aParentCaseActivityId";
	  public const string EXAMPLE_HISTORIC_CASE_ACTIVITY_ID = "aCaseActivityId";
	  public const string EXAMPLE_HISTORIC_ANOTHER_CASE_ACTIVITY_ID = "anotherCaseActivityId";
	  public const string EXAMPLE_HISTORIC_CASE_ACTIVITY_NAME = "aCaseActivityName";
	  public const string EXAMPLE_HISTORIC_CASE_ACTIVITY_TYPE = "aCaseActivityType";
	  public const string EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_CALLED_PROCESS_INSTANCE_ID = "aCalledProcessInstanceId";
	  public const string EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_CALLED_CASE_INSTANCE_ID = "aCalledCaseInstanceId";
	  public static readonly string EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_CREATE_TIME = withTimezone("2014-04-23T18:42:42");
	  public static readonly string EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_END_TIME = withTimezone("2014-04-23T18:42:43");
	  public const long EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_DURATION = 2000l;
	  public const bool EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_REQUIRED = true;
	  public const bool EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_AVAILABLE = true;
	  public const bool EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_ENABLED = true;
	  public const bool EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_DISABLED = true;
	  public const bool EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_ACTIVE = true;
	  public const bool EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_FAILED = true;
	  public const bool EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_SUSPENDED = true;
	  public const bool EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_COMPLETED = true;
	  public const bool EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_TERMINATED = true;
	  public const bool EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_UNFINISHED = true;
	  public const bool EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_FINISHED = true;

	  public static readonly string EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_CREATED_AFTER = withTimezone("2014-04-23T18:41:42");
	  public static readonly string EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_CREATED_BEFORE = withTimezone("2014-04-23T18:43:42");
	  public static readonly string EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_ENDED_AFTER = withTimezone("2014-04-23T18:41:43");
	  public static readonly string EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_ENDED_BEFORE = withTimezone("2014-04-23T18:43:43");

	  // user operation log
	  public const string EXAMPLE_USER_OPERATION_LOG_ID = "userOpLogId";
	  public const string EXAMPLE_USER_OPERATION_ID = "opId";
	  public const string EXAMPLE_USER_OPERATION_TYPE = org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CLAIM;
	  public const string EXAMPLE_USER_OPERATION_ENTITY = EntityTypes.TASK;
	  public const string EXAMPLE_USER_OPERATION_PROPERTY = "opProperty";
	  public const string EXAMPLE_USER_OPERATION_ORG_VALUE = "orgValue";
	  public const string EXAMPLE_USER_OPERATION_NEW_VALUE = "newValue";
	  public static readonly string EXAMPLE_USER_OPERATION_TIMESTAMP = withTimezone("2014-02-20T16:53:37");

	  // historic detail
	  public const string EXAMPLE_HISTORIC_VAR_UPDATE_ID = "aHistoricVariableUpdateId";
	  public const string EXAMPLE_HISTORIC_VAR_UPDATE_PROC_DEF_KEY = "aProcDefKey";
	  public const string EXAMPLE_HISTORIC_VAR_UPDATE_PROC_DEF_ID = "aProcDefId";
	  public const string EXAMPLE_HISTORIC_VAR_UPDATE_PROC_INST_ID = "aProcInst";
	  public const string EXAMPLE_HISTORIC_VAR_UPDATE_ACT_INST_ID = "anActInst";
	  public const string EXAMPLE_HISTORIC_VAR_UPDATE_EXEC_ID = "anExecutionId";
	  public const string EXAMPLE_HISTORIC_VAR_UPDATE_OPERATION_ID = "anOperationId";
	  public const string EXAMPLE_HISTORIC_VAR_UPDATE_TASK_ID = "aTaskId";
	  public static readonly string EXAMPLE_HISTORIC_VAR_UPDATE_TIME = withTimezone("2014-01-01T00:00:00");
	  public const string EXAMPLE_HISTORIC_VAR_UPDATE_NAME = "aVariableName";
	  public const string EXAMPLE_HISTORIC_VAR_UPDATE_TYPE_NAME = "String";
	  public const string EXAMPLE_HISTORIC_VAR_UPDATE_VALUE_TYPE_NAME = "String";
	  public const int EXAMPLE_HISTORIC_VAR_UPDATE_REVISION = 1;
	  public const string EXAMPLE_HISTORIC_VAR_UPDATE_ERROR = "anErrorMessage";
	  public const string EXAMPLE_HISTORIC_VAR_UPDATE_VAR_INST_ID = "aVariableInstanceId";
	  public const string EXAMPLE_HISTORIC_VAR_UPDATE_CASE_DEF_KEY = "aCaseDefKey";
	  public const string EXAMPLE_HISTORIC_VAR_UPDATE_CASE_DEF_ID = "aCaseDefId";
	  public const string EXAMPLE_HISTORIC_VAR_UPDATE_CASE_INST_ID = "aCaseInstId";
	  public const string EXAMPLE_HISTORIC_VAR_UPDATE_CASE_EXEC_ID = "aCaseExecId";

	  public const string EXAMPLE_HISTORIC_FORM_FIELD_ID = "anId";
	  public const string EXAMPLE_HISTORIC_FORM_FIELD_PROC_DEF_KEY = "aProcDefKey";
	  public const string EXAMPLE_HISTORIC_FORM_FIELD_PROC_DEF_ID = "aProcDefId";
	  public const string EXAMPLE_HISTORIC_FORM_FIELD_PROC_INST_ID = "aProcInst";
	  public const string EXAMPLE_HISTORIC_FORM_FIELD_ACT_INST_ID = "anActInst";
	  public const string EXAMPLE_HISTORIC_FORM_FIELD_EXEC_ID = "anExecutionId";
	  public const string EXAMPLE_HISTORIC_FORM_FIELD_OPERATION_ID = "anOperationId";
	  public const string EXAMPLE_HISTORIC_FORM_FIELD_TASK_ID = "aTaskId";
	  public static readonly string EXAMPLE_HISTORIC_FORM_FIELD_TIME = withTimezone("2014-01-01T00:00:00");
	  public const string EXAMPLE_HISTORIC_FORM_FIELD_FIELD_ID = "aFormFieldId";
	  public const string EXAMPLE_HISTORIC_FORM_FIELD_VALUE = "aFormFieldValue";
	  public const string EXAMPLE_HISTORIC_FORM_FIELD_CASE_DEF_KEY = "aCaseDefKey";
	  public const string EXAMPLE_HISTORIC_FORM_FIELD_CASE_DEF_ID = "aCaseDefId";
	  public const string EXAMPLE_HISTORIC_FORM_FIELD_CASE_INST_ID = "aCaseInstId";
	  public const string EXAMPLE_HISTORIC_FORM_FIELD_CASE_EXEC_ID = "aCaseExecId";
	  public const string EXAMPLE_HISTORIC_FORM_ROOT_PROCESS_INSTANCE_ID = "aRootProcInstId";

	  // historic task instance
	  public const string EXAMPLE_HISTORIC_TASK_INST_ID = "aHistoricTaskInstanceId";
	  public const string EXAMPLE_HISTORIC_TASK_INST_PROC_DEF_ID = "aProcDefId";
	  public const string EXAMPLE_HISTORIC_TASK_INST_PROC_DEF_KEY = "aProcDefKey";
	  public const string EXAMPLE_HISTORIC_TASK_INST_PROC_INST_ID = "aProcInstId";
	  public const string EXAMPLE_HISTORIC_TASK_INST_PROC_INST_BUSINESS_KEY = "aBusinessKey";
	  public const string EXAMPLE_HISTORIC_TASK_INST_EXEC_ID = "anExecId";
	  public const string EXAMPLE_HISTORIC_TASK_INST_ACT_INST_ID = "anActInstId";
	  public const string EXAMPLE_HISTORIC_TASK_INST_NAME = "aName";
	  public const string EXAMPLE_HISTORIC_TASK_INST_DESCRIPTION = "aDescription";
	  public const string EXAMPLE_HISTORIC_TASK_INST_DELETE_REASON = "aDeleteReason";
	  public const string EXAMPLE_HISTORIC_TASK_INST_OWNER = "anOwner";
	  public const string EXAMPLE_HISTORIC_TASK_INST_ASSIGNEE = "anAssignee";
	  public static readonly string EXAMPLE_HISTORIC_TASK_INST_START_TIME = withTimezone("2014-01-01T00:00:00");
	  public static readonly string EXAMPLE_HISTORIC_TASK_INST_END_TIME = withTimezone("2014-01-01T00:00:00");
	  public static readonly string EXAMPLE_HISTORIC_TASK_INST_REMOVAL_TIME = withTimezone("2018-01-01T00:00:00");
	  public const long? EXAMPLE_HISTORIC_TASK_INST_DURATION = 5000L;
	  public const string EXAMPLE_HISTORIC_TASK_INST_DEF_KEY = "aTaskDefinitionKey";
	  public const int EXAMPLE_HISTORIC_TASK_INST_PRIORITY = 60;
	  public static readonly string EXAMPLE_HISTORIC_TASK_INST_DUE_DATE = withTimezone("2014-01-01T00:00:00");
	  public static readonly string EXAMPLE_HISTORIC_TASK_INST_FOLLOW_UP_DATE = withTimezone("2014-01-01T00:00:00");
	  public const string EXAMPLE_HISTORIC_TASK_INST_PARENT_TASK_ID = "aParentTaskId";
	  public const string EXAMPLE_HISTORIC_TASK_INST_CASE_DEF_KEY = "aCaseDefinitionKey";
	  public const string EXAMPLE_HISTORIC_TASK_INST_CASE_DEF_ID = "aCaseDefinitionId";
	  public const string EXAMPLE_HISTORIC_TASK_INST_CASE_INST_ID = "aCaseInstanceId";
	  public const string EXAMPLE_HISTORIC_TASK_INST_CASE_EXEC_ID = "aCaseExecutionId";
	  public const string EXAMPLE_HISTORIC_TASK_INST_TASK_INVOLVED_USER = "aUserId";
	  public const string EXAMPLE_HISTORIC_TASK_INST_TASK_INVOLVED_GROUP = "aGroupId";
	  public const string EXAMPLE_HISTORIC_TASK_INST_TASK_HAD_CANDIDATE_USER = "cUserId";
	  public const string EXAMPLE_HISTORIC_TASK_INST_TASK_HAD_CANDIDATE_GROUP = "cGroupId";
	  public const string EXAMPLE_HISTORIC_TASK_INST_ROOT_PROC_INST_ID = "aRootProcInstId";

	  // Incident
	  public const string EXAMPLE_INCIDENT_ID = "anIncidentId";
	  public static readonly string EXAMPLE_INCIDENT_TIMESTAMP = withTimezone("2014-01-01T00:00:00");
	  public const string EXAMPLE_INCIDENT_TYPE = "anIncidentType";
	  public const string EXAMPLE_INCIDENT_EXECUTION_ID = "anExecutionId";
	  public const string EXAMPLE_INCIDENT_ACTIVITY_ID = "anActivityId";
	  public const string EXAMPLE_INCIDENT_PROC_INST_ID = "aProcInstId";
	  public const string EXAMPLE_INCIDENT_PROC_DEF_ID = "aProcDefId";
	  public const string EXAMPLE_INCIDENT_CAUSE_INCIDENT_ID = "aCauseIncidentId";
	  public const string EXAMPLE_INCIDENT_ROOT_CAUSE_INCIDENT_ID = "aRootCauseIncidentId";
	  public const string EXAMPLE_INCIDENT_CONFIGURATION = "aConfiguration";
	  public const string EXAMPLE_INCIDENT_MESSAGE = "anIncidentMessage";
	  public const string EXAMPLE_INCIDENT_MESSAGE_LIKE = "%anIncidentMessageLike%";

	  public const int EXAMPLE_INCIDENT_COUNT = 1;

	  // Historic Incident
	  public const string EXAMPLE_HIST_INCIDENT_ID = "anIncidentId";
	  public static readonly string EXAMPLE_HIST_INCIDENT_CREATE_TIME = withTimezone("2014-01-01T00:00:00");
	  public static readonly string EXAMPLE_HIST_INCIDENT_END_TIME = withTimezone("2014-01-01T00:00:00");
	  public static readonly string EXAMPLE_HIST_INCIDENT_REMOVAL_TIME = withTimezone("2018-01-01T00:00:00");
	  public const string EXAMPLE_HIST_INCIDENT_TYPE = "anIncidentType";
	  public const string EXAMPLE_HIST_INCIDENT_EXECUTION_ID = "anExecutionId";
	  public const string EXAMPLE_HIST_INCIDENT_ACTIVITY_ID = "anActivityId";
	  public const string EXAMPLE_HIST_INCIDENT_PROC_INST_ID = "aProcInstId";
	  public const string EXAMPLE_HIST_INCIDENT_PROC_DEF_ID = "aProcDefId";
	  public const string EXAMPLE_HIST_INCIDENT_PROC_DEF_KEY = "aProcDefKey";
	  public const string EXAMPLE_HIST_INCIDENT_ROOT_PROC_INST_ID = "aRootProcInstId";
	  public const string EXAMPLE_HIST_INCIDENT_CAUSE_INCIDENT_ID = "aCauseIncidentId";
	  public const string EXAMPLE_HIST_INCIDENT_ROOT_CAUSE_INCIDENT_ID = "aRootCauseIncidentId";
	  public const string EXAMPLE_HIST_INCIDENT_CONFIGURATION = "aConfiguration";
	  public const string EXAMPLE_HIST_INCIDENT_MESSAGE = "anIncidentMessage";
	  public const bool EXAMPLE_HIST_INCIDENT_STATE_OPEN = false;
	  public const bool EXAMPLE_HIST_INCIDENT_STATE_DELETED = false;
	  public const bool EXAMPLE_HIST_INCIDENT_STATE_RESOLVED = true;

	  // Historic Identity Link
	  public const string EXAMPLE_HIST_IDENTITY_LINK_TYPE = "assignee";
	  public const string EXAMPLE_HIST_IDENTITY_LINK_OPERATION_TYPE = "add";
	  public static readonly string EXAMPLE_HIST_IDENTITY_LINK_TIME = withTimezone("2014-01-05T00:00:00");
	  public static readonly string EXAMPLE_HIST_IDENTITY_LINK_REMOVAL_TIME = withTimezone("2018-01-05T00:00:00");
	  public static readonly string EXAMPLE_HIST_IDENTITY_LINK_DATE_BEFORE = withTimezone("2014-01-01T00:00:00");
	  public static readonly string EXAMPLE_HIST_IDENTITY_LINK_DATE_AFTER = withTimezone("2014-01-06T00:00:00");
	  public const string EXAMPLE_HIST_IDENTITY_LINK_ASSIGNER_ID = "aAssignerId";
	  public const string EXAMPLE_HIST_IDENTITY_LINK_TASK_ID = "aTaskId";
	  public const string EXAMPLE_HIST_IDENTITY_LINK_USER_ID = "aUserId";
	  public const string EXAMPLE_HIST_IDENTITY_LINK_GROUP_ID = "aGroupId";
	  public const string EXAMPLE_HIST_IDENTITY_LINK_PROC_DEFINITION_ID = "aProcDefId";
	  public const string EXAMPLE_HIST_IDENTITY_LINK_PROC_DEFINITION_KEY = "aProcDefKey";
	  public const string EXAMPLE_HIST_IDENTITY_LINK_ROOT_PROC_INST_ID = "aRootProcInstId";

	  // case definition
	  public const string EXAMPLE_CASE_DEFINITION_ID = "aCaseDefnitionId";
	  public const string ANOTHER_EXAMPLE_CASE_DEFINITION_ID = "anotherCaseDefnitionId";
	  public static readonly string EXAMPLE_CASE_DEFINITION_ID_LIST = EXAMPLE_CASE_DEFINITION_ID + "," + ANOTHER_EXAMPLE_CASE_DEFINITION_ID;
	  public const string EXAMPLE_CASE_DEFINITION_KEY = "aCaseDefinitionKey";
	  public const int EXAMPLE_CASE_DEFINITION_VERSION = 1;
	  public const string EXAMPLE_CASE_DEFINITION_CATEGORY = "aCaseDefinitionCategory";
	  public const string EXAMPLE_CASE_DEFINITION_NAME = "aCaseDefinitionName";
	  public const string EXAMPLE_CASE_DEFINITION_NAME_LIKE = "aCaseDefinitionNameLike";
	  public const string EXAMPLE_CASE_DEFINITION_RESOURCE_NAME = "aCaseDefinitionResourceName";
	  public const string EXAMPLE_CASE_DEFINITION_DIAGRAM_RESOURCE_NAME = "aResourceName.png";

	  // case instance
	  public const string EXAMPLE_CASE_INSTANCE_ID = "aCaseInstId";
	  public const string EXAMPLE_CASE_INSTANCE_BUSINESS_KEY = "aBusinessKey";
	  public const string EXAMPLE_CASE_INSTANCE_BUSINESS_KEY_LIKE = "aBusinessKeyLike";
	  public const string EXAMPLE_CASE_INSTANCE_CASE_DEFINITION_ID = "aCaseDefinitionId";
	  public const bool EXAMPLE_CASE_INSTANCE_IS_ACTIVE = true;
	  public const bool EXAMPLE_CASE_INSTANCE_IS_COMPLETED = true;
	  public const bool EXAMPLE_CASE_INSTANCE_IS_TERMINATED = true;

	  // case execution
	  public const string EXAMPLE_CASE_EXECUTION_ID = "aCaseExecutionId";
	  public const string ANOTHER_EXAMPLE_CASE_EXECUTION_ID = "anotherCaseExecutionId";
	  public const string EXAMPLE_CASE_EXECUTION_CASE_INSTANCE_ID = "aCaseInstanceId";
	  public const string EXAMPLE_CASE_EXECUTION_PARENT_ID = "aParentId";
	  public const string EXAMPLE_CASE_EXECUTION_CASE_DEFINITION_ID = "aCaseDefinitionId";
	  public const string EXAMPLE_CASE_EXECUTION_ACTIVITY_ID = "anActivityId";
	  public const string EXAMPLE_CASE_EXECUTION_ACTIVITY_NAME = "anActivityName";
	  public const string EXAMPLE_CASE_EXECUTION_ACTIVITY_TYPE = "anActivityType";
	  public const string EXAMPLE_CASE_EXECUTION_ACTIVITY_DESCRIPTION = "anActivityDescription";
	  public const bool EXAMPLE_CASE_EXECUTION_IS_REQUIRED = true;
	  public const bool EXAMPLE_CASE_EXECUTION_IS_ENABLED = true;
	  public const bool EXAMPLE_CASE_EXECUTION_IS_ACTIVE = true;
	  public const bool EXAMPLE_CASE_EXECUTION_IS_DISABLED = true;

	  // filter
	  public const string EXAMPLE_FILTER_ID = "aFilterId";
	  public const string ANOTHER_EXAMPLE_FILTER_ID = "anotherFilterId";
	  public const string EXAMPLE_FILTER_RESOURCE_TYPE = EntityTypes.TASK;
	  public const string EXAMPLE_FILTER_NAME = "aFilterName";
	  public const string EXAMPLE_FILTER_OWNER = "aFilterOwner";
	  public static readonly Query EXAMPLE_FILTER_QUERY = new TaskQueryImpl().taskName("test").processVariableValueEquals("foo", "bar").caseInstanceVariableValueEquals("foo", "bar").taskVariableValueEquals("foo", "bar");
	  public static readonly TaskQueryDto EXAMPLE_FILTER_QUERY_DTO = TaskQueryDto.fromQuery(EXAMPLE_FILTER_QUERY);
	  public static readonly IDictionary<string, object> EXAMPLE_FILTER_PROPERTIES = Collections.singletonMap("color", (object) "#112233");

	  // decision definition
	  public const string EXAMPLE_DECISION_DEFINITION_ID_IN = "aDecisionDefinitionId,anotherDecisionDefinitionId";
	  public const string EXAMPLE_DECISION_DEFINITION_ID = "aDecisionDefinitionId";
	  public const string ANOTHER_EXAMPLE_DECISION_DEFINITION_ID = "anotherDecisionDefinitionId";
	  public static readonly string EXAMPLE_DECISION_DEFINITION_ID_LIST = EXAMPLE_DECISION_DEFINITION_ID + "," + ANOTHER_EXAMPLE_DECISION_DEFINITION_ID;
	  public const string EXAMPLE_DECISION_DEFINITION_KEY = "aDecisionDefinitionKey";
	  public const string ANOTHER_DECISION_DEFINITION_KEY = "anotherDecisionDefinitionKey";
	  public const string EXAMPLE_DECISION_DEFINITION_KEY_IN = "aDecisionDefinitionKey,anotherDecisionDefinitionKey";
	  public const int EXAMPLE_DECISION_DEFINITION_VERSION = 1;
	  public const string EXAMPLE_DECISION_DEFINITION_CATEGORY = "aDecisionDefinitionCategory";
	  public const string EXAMPLE_DECISION_DEFINITION_NAME = "aDecisionDefinitionName";
	  public const string EXAMPLE_DECISION_DEFINITION_NAME_LIKE = "aDecisionDefinitionNameLike";
	  public const string EXAMPLE_DECISION_DEFINITION_RESOURCE_NAME = "aDecisionDefinitionResourceName";
	  public const string EXAMPLE_DECISION_DEFINITION_DIAGRAM_RESOURCE_NAME = "aResourceName.png";

	  public const string EXAMPLE_DECISION_OUTPUT_KEY = "aDecisionOutput";
	  public static readonly StringValue EXAMPLE_DECISION_OUTPUT_VALUE = Variables.stringValue("aDecisionOutputValue");

	  // decision requirement definition
	  public const string EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID = "aDecisionRequirementsDefinitionId";
	  public const string EXAMPLE_DECISION_INSTANCE_ID = "aDecisionInstanceId";
	  public const string ANOTHER_EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID = "anotherDecisionRequirementsDefinitionId";
	  public static readonly string EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID_LIST = EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID + "," + ANOTHER_EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID;
	  public const string EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY = "aDecisionRequirementsDefinitionKey";
	  public const int EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_VERSION = 1;
	  public const string EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_CATEGORY = "aDecisionRequirementsDefinitionCategory";
	  public const string EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_NAME = "aDecisionRequirementsDefinitionName";
	  public const string EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_RESOURCE_NAME = "aDecisionRequirementsDefinitionResourceName";
	  public const string EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_DIAGRAM_RESOURCE_NAME = "aResourceName.png";

	  // historic job log

	  public const string EXAMPLE_HISTORIC_JOB_LOG_ID = "aHistoricJobLogId";
	  public static readonly string EXAMPLE_HISTORIC_JOB_LOG_TIMESTAMP = withTimezone(withTimezone("2015-01-01T00:00:00"));
	  public static readonly string EXAMPLE_HISTORIC_JOB_LOG_REMOVAL_TIME = withTimezone(withTimezone("2018-01-01T00:00:00"));

	  public const string EXAMPLE_HISTORIC_JOB_LOG_JOB_ID = "aJobId";
	  public static readonly string EXAMPLE_HISTORIC_JOB_LOG_JOB_DUE_DATE = withTimezone("2015-10-01T00:00:00");
	  public const int EXAMPLE_HISTORIC_JOB_LOG_JOB_RETRIES = 5;
	  public static readonly long EXAMPLE_HISTORIC_JOB_LOG_JOB_PRIORITY = int.MaxValue + 42l;
	  public const string EXAMPLE_HISTORIC_JOB_LOG_JOB_EXCEPTION_MSG = "aJobExceptionMsg";
	  public const string EXAMPLE_HISTORIC_JOB_LOG_JOB_DEF_ID = "aJobDefId";
	  public const string EXAMPLE_HISTORIC_JOB_LOG_JOB_DEF_TYPE = "aJobDefType";
	  public const string EXAMPLE_HISTORIC_JOB_LOG_JOB_DEF_CONFIG = "aJobDefConfig";
	  public const string EXAMPLE_HISTORIC_JOB_LOG_ACTIVITY_ID = "anActId";
	  public const string EXAMPLE_HISTORIC_JOB_LOG_EXECUTION_ID = "anExecId";
	  public const string EXAMPLE_HISTORIC_JOB_LOG_PROC_INST_ID = "aProcInstId";
	  public const string EXAMPLE_HISTORIC_JOB_LOG_PROC_DEF_ID = "aProcDefId";
	  public const string EXAMPLE_HISTORIC_JOB_LOG_PROC_DEF_KEY = "aProcDefKey";
	  public const string EXAMPLE_HISTORIC_JOB_LOG_DEPLOYMENT_ID = "aDeploymentId";
	  public const string EXAMPLE_HISTORIC_JOB_LOG_ROOT_PROC_INST_ID = "aRootProcInstId";
	  public const bool EXAMPLE_HISTORIC_JOB_LOG_IS_CREATION_LOG = true;
	  public const bool EXAMPLE_HISTORIC_JOB_LOG_IS_FAILURE_LOG = true;
	  public const bool EXAMPLE_HISTORIC_JOB_LOG_IS_SUCCESS_LOG = true;
	  public const bool EXAMPLE_HISTORIC_JOB_LOG_IS_DELETION_LOG = true;

	  // historic decision instance
	  public const string EXAMPLE_HISTORIC_DECISION_INSTANCE_ID = "aHistoricDecisionInstanceId";
	  public const string EXAMPLE_HISTORIC_DECISION_INSTANCE_ID_IN = "aHistoricDecisionInstanceId,anotherHistoricDecisionInstanceId";
	  public const string EXAMPLE_HISTORIC_DECISION_INSTANCE_ACTIVITY_ID = "aHistoricDecisionInstanceActivityId";
	  public const string EXAMPLE_HISTORIC_DECISION_INSTANCE_ACTIVITY_ID_IN = "aHistoricDecisionInstanceActivityId,anotherHistoricDecisionInstanceActivityId";
	  public const string EXAMPLE_HISTORIC_DECISION_INSTANCE_ACTIVITY_INSTANCE_ID = "aHistoricDecisionInstanceActivityInstanceId";
	  public const string EXAMPLE_HISTORIC_DECISION_INSTANCE_ACTIVITY_INSTANCE_ID_IN = "aHistoricDecisionInstanceActivityInstanceId,anotherHistoricDecisionInstanceActivityInstanceId";
	  public static readonly string EXAMPLE_HISTORIC_DECISION_INSTANCE_EVALUATION_TIME = withTimezone("2015-09-07T11:00:00");
	  public static readonly string EXAMPLE_HISTORIC_DECISION_INSTANCE_REMOVAL_TIME = withTimezone("2015-09-10T11:00:00");
	  public static readonly string EXAMPLE_HISTORIC_DECISION_INSTANCE_EVALUATED_BEFORE = withTimezone("2015-09-08T11:00:00");
	  public static readonly string EXAMPLE_HISTORIC_DECISION_INSTANCE_EVALUATED_AFTER = withTimezone("2015-09-06T11:00:00");
	  public const string EXAMPLE_HISTORIC_DECISION_INSTANCE_USER_ID = "aUserId";
	  public const double? EXAMPLE_HISTORIC_DECISION_INSTANCE_COLLECT_RESULT_VALUE = 42.0;
	  public const string EXAMPLE_HISTORIC_DECISION_INPUT_INSTANCE_ID = "aDecisionInputInstanceId";
	  public const string EXAMPLE_HISTORIC_DECISION_INPUT_INSTANCE_CLAUSE_ID = "aDecisionInputClauseId";
	  public const string EXAMPLE_HISTORIC_DECISION_INPUT_INSTANCE_CLAUSE_NAME = "aDecisionInputClauseName";
	  public static readonly string EXAMPLE_HISTORIC_DECISION_INPUT_INSTANCE_CREATE_TIME = withTimezone("2015-09-06T11:00:00");
	  public static readonly string EXAMPLE_HISTORIC_DECISION_INPUT_INSTANCE_REMOVAL_TIME = withTimezone("2015-10-18T11:00:00");
	  public const string EXAMPLE_HISTORIC_DECISION_INPUT_ROOT_PROCESS_INSTANCE_ID = "aRootProcInstId";
	  public const string EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_ID = "aDecisionInputInstanceId";
	  public const string EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_VARIABLE_NAME = "aDecisionInputInstanceName";
	  public const string EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_CLAUSE_ID = "aDecisionInputClauseId";
	  public const string EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_CLAUSE_NAME = "aDecisionInputClauseName";
	  public const string EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_RULE_ID = "aDecisionInputRuleId";
	  public const int? EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_RULE_ORDER = 12;
	  public static readonly string EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_CREATE_TIME = withTimezone("2015-09-06T11:00:00");
	  public static readonly string EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_REMOVAL_TIME = withTimezone("2015-10-18T11:00:00");
	  public const string EXAMPLE_HISTORIC_DECISION_OUTPUT_ROOT_PROCESS_INSTANCE_ID = "aRootProcInstId";
	  public static readonly ObjectValue EXAMPLE_HISTORIC_DECISION_SERIALIZED_VALUE = MockObjectValue.fromObjectValue(Variables.objectValue("test").serializationDataFormat("aDataFormat").create()).objectTypeName("aTypeName");
	  public static readonly BytesValue EXAMPLE_HISTORIC_DECISION_BYTE_ARRAY_VALUE = Variables.byteArrayValue("test".Bytes);
	  public static readonly StringValue EXAMPLE_HISTORIC_DECISION_STRING_VALUE = Variables.stringValue("test");

	  // metrics
	  public static readonly string EXAMPLE_METRICS_START_DATE = withTimezone("2015-01-01T00:00:00");
	  public static readonly string EXAMPLE_METRICS_END_DATE = withTimezone("2015-02-01T00:00:00");
	  public const string EXAMPLE_METRICS_REPORTER = "REPORTER";
	  public const string EXAMPLE_METRICS_NAME = "metricName";

	  // external task
	  public const string EXTERNAL_TASK_ID = "anExternalTaskId";
	  public const string EXTERNAL_TASK_ERROR_MESSAGE = "some error";
	  public static readonly string EXTERNAL_TASK_LOCK_EXPIRATION_TIME = withTimezone("2015-10-05T13:25:00");
	  public static readonly int? EXTERNAL_TASK_RETRIES = new int?(5);
	  public const bool EXTERNAL_TASK_SUSPENDED = true;
	  public const string EXTERNAL_TASK_TOPIC_NAME = "aTopic";
	  public const string EXTERNAL_TASK_WORKER_ID = "aWorkerId";
	  public static readonly long EXTERNAL_TASK_PRIORITY = int.MaxValue + 466L;

	  // batch
	  public const string EXAMPLE_BATCH_ID = "aBatchId";
	  public const string EXAMPLE_BATCH_TYPE = "aBatchType";
	  public const int EXAMPLE_BATCH_TOTAL_JOBS = 10;
	  public const int EXAMPLE_BATCH_JOBS_CREATED = 9;
	  public const int EXAMPLE_BATCH_JOBS_PER_SEED = 11;
	  public const int EXAMPLE_INVOCATIONS_PER_BATCH_JOB = 12;
	  public const string EXAMPLE_SEED_JOB_DEFINITION_ID = "aSeedJobDefinitionId";
	  public const string EXAMPLE_MONITOR_JOB_DEFINITION_ID = "aMonitorJobDefinitionId";
	  public const string EXAMPLE_BATCH_JOB_DEFINITION_ID = "aBatchJobDefinitionId";
	  public static readonly string EXAMPLE_HISTORIC_BATCH_START_TIME = withTimezone("2016-04-12T15:29:33");
	  public static readonly string EXAMPLE_HISTORIC_BATCH_END_TIME = withTimezone("2016-04-12T16:23:34");
	  public static readonly string EXAMPLE_HISTORIC_BATCH_REMOVAL_TIME = withTimezone("2016-04-12T16:23:34");
	  public const int EXAMPLE_BATCH_REMAINING_JOBS = 21;
	  public const int EXAMPLE_BATCH_COMPLETED_JOBS = 22;
	  public const int EXAMPLE_BATCH_FAILED_JOBS = 23;

	  // tasks
	  public const long? EXAMPLE_HISTORIC_TASK_REPORT_COUNT = 12L;
	  public const string EXAMPLE_HISTORIC_TASK_REPORT_DEFINITION = "aTaskDefinition";
	  public const string EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEFINITION = "aProcessDefinition";
	  public static readonly string EXAMPLE_HISTORIC_TASK_START_TIME = withTimezone("2016-04-12T15:29:33");
	  public static readonly string EXAMPLE_HISTORIC_TASK_END_TIME = withTimezone("2016-04-12T16:23:34");
	  public const string EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEF_ID = "aProcessDefinitionId:1:1";
	  public const string EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEF_NAME = "aProcessDefinitionName";
	  public const string EXAMPLE_HISTORIC_TASK_REPORT_TASK_NAME = "aTaskName";

	  // historic task instance duration report
	  public const long EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_AVG = 10;
	  public const long EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_MIN = 5;
	  public const long EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_MAX = 15;
	  public const int EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_PERIOD = 1;

	  // historic external task log
	  public const string EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ID = "aHistoricExternalTaskLogId";
	  public static readonly string EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_TIMESTAMP = withTimezone("2015-01-01T00:00:00");
	  public static readonly string EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_REMOVAL_TIME = withTimezone("2018-01-01T00:00:00");
	  public const string EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_EXTERNAL_TASK_ID = "anExternalTaskId";
	  public const string EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_TOPIC_NAME = "aTopicName";
	  public const string EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_WORKER_ID = "aWorkerId";
	  public const int EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_RETRIES = 5;
	  public static readonly long EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PRIORITY = int.MaxValue + 42l;
	  public const string EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ERROR_MSG = "aEXTERNAL_TASKExceptionMsg";
	  public const string EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ACTIVITY_ID = "anActId";
	  public const string EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ACTIVITY_INSTANCE_ID = "anActInstanceId";
	  public const string EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_EXECUTION_ID = "anExecId";
	  public const string EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_INST_ID = "aProcInstId";
	  public const string EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_DEF_ID = "aProcDefId";
	  public const string EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_DEF_KEY = "aProcDefKey";
	  public const string EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ROOT_PROC_INST_ID = "aRootProcInstId";
	  public const bool EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_CREATION_LOG = true;
	  public const bool EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_FAILURE_LOG = true;
	  public const bool EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_SUCCESS_LOG = true;
	  public const bool EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_DELETION_LOG = true;
	  public static readonly string EXAMPLE_JOB_CREATE_TIME = withTimezone("2015-01-01T00:00:00");

	  public static Task createMockTask()
	  {
		return mockTask().build();
	  }

	  public static MockTaskBuilder mockTask()
	  {
		return (new MockTaskBuilder()).id(EXAMPLE_TASK_ID).name(EXAMPLE_TASK_NAME).assignee(EXAMPLE_TASK_ASSIGNEE_NAME).createTime(DateTimeUtil.parseDate(EXAMPLE_TASK_CREATE_TIME)).dueDate(DateTimeUtil.parseDate(EXAMPLE_TASK_DUE_DATE)).followUpDate(DateTimeUtil.parseDate(EXAMPLE_FOLLOW_UP_DATE)).delegationState(EXAMPLE_TASK_DELEGATION_STATE).description(EXAMPLE_TASK_DESCRIPTION).executionId(EXAMPLE_TASK_EXECUTION_ID).owner(EXAMPLE_TASK_OWNER).parentTaskId(EXAMPLE_TASK_PARENT_TASK_ID).priority(EXAMPLE_TASK_PRIORITY).processDefinitionId(EXAMPLE_PROCESS_DEFINITION_ID).processInstanceId(EXAMPLE_PROCESS_INSTANCE_ID).taskDefinitionKey(EXAMPLE_TASK_DEFINITION_KEY).caseDefinitionId(EXAMPLE_CASE_DEFINITION_ID).caseInstanceId(EXAMPLE_CASE_INSTANCE_ID).caseExecutionId(EXAMPLE_CASE_EXECUTION_ID).formKey(EXAMPLE_FORM_KEY).tenantId(EXAMPLE_TENANT_ID);
	  }

	  public static IList<Task> createMockTasks()
	  {
		IList<Task> mocks = new List<Task>();
		mocks.Add(createMockTask());
		return mocks;
	  }

	  public static TaskFormData createMockTaskFormData()
	  {
		FormProperty mockFormProperty = mock(typeof(FormProperty));
		when(mockFormProperty.Id).thenReturn(EXAMPLE_FORM_PROPERTY_ID);
		when(mockFormProperty.Name).thenReturn(EXAMPLE_FORM_PROPERTY_NAME);
		when(mockFormProperty.Value).thenReturn(EXAMPLE_FORM_PROPERTY_VALUE);
		when(mockFormProperty.Readable).thenReturn(EXAMPLE_FORM_PROPERTY_READABLE);
		when(mockFormProperty.Writable).thenReturn(EXAMPLE_FORM_PROPERTY_WRITABLE);
		when(mockFormProperty.Required).thenReturn(EXAMPLE_FORM_PROPERTY_REQUIRED);

		FormType mockFormType = mock(typeof(FormType));
		when(mockFormType.Name).thenReturn(EXAMPLE_FORM_PROPERTY_TYPE_NAME);
		when(mockFormProperty.Type).thenReturn(mockFormType);

		TaskFormData mockFormData = mock(typeof(TaskFormData));
		when(mockFormData.FormKey).thenReturn(EXAMPLE_FORM_KEY);
		when(mockFormData.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);

		IList<FormProperty> mockFormProperties = new List<FormProperty>();
		mockFormProperties.Add(mockFormProperty);
		when(mockFormData.FormProperties).thenReturn(mockFormProperties);
		return mockFormData;
	  }

	  public static TaskFormData createMockTaskFormDataUsingFormFieldsWithoutFormKey()
	  {
		FormField mockFormField = mock(typeof(FormField));
		when(mockFormField.Id).thenReturn(EXAMPLE_FORM_PROPERTY_ID);
		when(mockFormField.Label).thenReturn(EXAMPLE_FORM_PROPERTY_NAME);
		when(mockFormField.DefaultValue).thenReturn(EXAMPLE_FORM_PROPERTY_VALUE);

		FormType mockFormType = mock(typeof(FormType));
		when(mockFormType.Name).thenReturn(EXAMPLE_FORM_PROPERTY_TYPE_NAME);
		when(mockFormField.Type).thenReturn(mockFormType);

		TaskFormData mockFormData = mock(typeof(TaskFormData));
		when(mockFormData.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);

		IList<FormField> mockFormFields = new List<FormField>();
		mockFormFields.Add(mockFormField);
		when(mockFormData.FormFields).thenReturn(mockFormFields);
		return mockFormData;
	  }

	  // task comment
	  public static Comment createMockTaskComment()
	  {
		Comment mockComment = mock(typeof(Comment));
		when(mockComment.Id).thenReturn(EXAMPLE_TASK_COMMENT_ID);
		when(mockComment.TaskId).thenReturn(EXAMPLE_TASK_ID);
		when(mockComment.UserId).thenReturn(EXAMPLE_USER_ID);
		when(mockComment.Time).thenReturn(DateTimeUtil.parseDate(EXAMPLE_TASK_COMMENT_TIME));
		when(mockComment.FullMessage).thenReturn(EXAMPLE_TASK_COMMENT_FULL_MESSAGE);
		when(mockComment.RemovalTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_TASK_COMMENT_TIME));
		when(mockComment.RootProcessInstanceId).thenReturn(EXAMPLE_TASK_COMMENT_ROOT_PROCESS_INSTANCE_ID);
		return mockComment;
	  }

	  public static IList<Comment> createMockTaskComments()
	  {
		IList<Comment> mocks = new List<Comment>();
		mocks.Add(createMockTaskComment());
		return mocks;
	  }

	  // task attachment
	  public static Attachment createMockTaskAttachment()
	  {
		Attachment mockAttachment = mock(typeof(Attachment));
		when(mockAttachment.Id).thenReturn(EXAMPLE_TASK_ATTACHMENT_ID);
		when(mockAttachment.Name).thenReturn(EXAMPLE_TASK_ATTACHMENT_NAME);
		when(mockAttachment.Description).thenReturn(EXAMPLE_TASK_ATTACHMENT_DESCRIPTION);
		when(mockAttachment.Type).thenReturn(EXAMPLE_TASK_ATTACHMENT_TYPE);
		when(mockAttachment.Url).thenReturn(EXAMPLE_TASK_ATTACHMENT_URL);
		when(mockAttachment.TaskId).thenReturn(EXAMPLE_TASK_ID);
		when(mockAttachment.ProcessInstanceId).thenReturn(EXAMPLE_PROCESS_INSTANCE_ID);
		when(mockAttachment.CreateTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_TASK_ATTACHMENT_CREATE_DATE));
		when(mockAttachment.RemovalTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_TASK_ATTACHMENT_REMOVAL_DATE));
		when(mockAttachment.RootProcessInstanceId).thenReturn(EXAMPLE_TASK_ATTACHMENT_ROOT_PROCESS_INSTANCE_ID);

		return mockAttachment;
	  }

	  public static IList<Attachment> createMockTaskAttachments()
	  {
		IList<Attachment> mocks = new List<Attachment>();
		mocks.Add(createMockTaskAttachment());
		return mocks;
	  }

	  public static IList<TaskCountByCandidateGroupResult> createMockTaskCountByCandidateGroupReport()
	  {
		TaskCountByCandidateGroupResult mock = mock(typeof(TaskCountByCandidateGroupResult));
		when(mock.GroupName).thenReturn(EXAMPLE_GROUP_ID);
		when(mock.TaskCount).thenReturn(EXAMPLE_TASK_COUNT_BY_CANDIDATE_GROUP);

		IList<TaskCountByCandidateGroupResult> mockList = new List<TaskCountByCandidateGroupResult>();
		mockList.Add(mock);
		return mockList;
	  }

	  public static IList<HistoricTaskInstanceReportResult> createMockHistoricTaskInstanceReport()
	  {
		HistoricTaskInstanceReportResult mock = mock(typeof(HistoricTaskInstanceReportResult));
		when(mock.Count).thenReturn(EXAMPLE_HISTORIC_TASK_REPORT_COUNT);
		when(mock.ProcessDefinitionId).thenReturn(EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEF_ID);
		when(mock.ProcessDefinitionKey).thenReturn(EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEFINITION);
		when(mock.ProcessDefinitionName).thenReturn(EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEF_NAME);
		when(mock.TenantId).thenReturn(EXAMPLE_TENANT_ID);
		when(mock.TaskName).thenReturn(EXAMPLE_HISTORIC_TASK_REPORT_TASK_NAME);

		return Collections.singletonList(mock);
	  }

	  public static IList<HistoricTaskInstanceReportResult> createMockHistoricTaskInstanceReportWithProcDef()
	  {
		HistoricTaskInstanceReportResult mock = mock(typeof(HistoricTaskInstanceReportResult));
		when(mock.Count).thenReturn(EXAMPLE_HISTORIC_TASK_REPORT_COUNT);
		when(mock.ProcessDefinitionId).thenReturn(EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEF_ID);
		when(mock.ProcessDefinitionKey).thenReturn(EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEFINITION);
		when(mock.ProcessDefinitionName).thenReturn(EXAMPLE_HISTORIC_TASK_REPORT_PROC_DEF_NAME);
		when(mock.TenantId).thenReturn(EXAMPLE_TENANT_ID);
		when(mock.TaskName).thenReturn(null);

		return Collections.singletonList(mock);
	  }

	  public static IList<DurationReportResult> createMockHistoricTaskInstanceDurationReport(PeriodUnit periodUnit)
	  {
		DurationReportResult mock = mock(typeof(DurationReportResult));
		when(mock.Average).thenReturn(EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_AVG);
		when(mock.Minimum).thenReturn(EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_MIN);
		when(mock.Maximum).thenReturn(EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_MAX);
		when(mock.Period).thenReturn(EXAMPLE_HISTORIC_TASK_INST_DURATION_REPORT_PERIOD);
		when(mock.PeriodUnit).thenReturn(periodUnit);

		IList<DurationReportResult> mockList = new List<DurationReportResult>();
		mockList.Add(mock);
		return mockList;
	  }


	  // form data
	  public static StartFormData createMockStartFormData(ProcessDefinition definition)
	  {
		FormProperty mockFormProperty = mock(typeof(FormProperty));
		when(mockFormProperty.Id).thenReturn(EXAMPLE_FORM_PROPERTY_ID);
		when(mockFormProperty.Name).thenReturn(EXAMPLE_FORM_PROPERTY_NAME);
		when(mockFormProperty.Value).thenReturn(EXAMPLE_FORM_PROPERTY_VALUE);
		when(mockFormProperty.Readable).thenReturn(EXAMPLE_FORM_PROPERTY_READABLE);
		when(mockFormProperty.Writable).thenReturn(EXAMPLE_FORM_PROPERTY_WRITABLE);
		when(mockFormProperty.Required).thenReturn(EXAMPLE_FORM_PROPERTY_REQUIRED);

		FormType mockFormType = mock(typeof(FormType));
		when(mockFormType.Name).thenReturn(EXAMPLE_FORM_PROPERTY_TYPE_NAME);
		when(mockFormProperty.Type).thenReturn(mockFormType);

		StartFormData mockFormData = mock(typeof(StartFormData));
		when(mockFormData.FormKey).thenReturn(EXAMPLE_FORM_KEY);
		when(mockFormData.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		when(mockFormData.ProcessDefinition).thenReturn(definition);

		IList<FormProperty> mockFormProperties = new List<FormProperty>();
		mockFormProperties.Add(mockFormProperty);
		when(mockFormData.FormProperties).thenReturn(mockFormProperties);
		return mockFormData;
	  }

	  public static StartFormData createMockStartFormDataUsingFormFieldsWithoutFormKey(ProcessDefinition definition)
	  {
		FormField mockFormField = mock(typeof(FormField));
		when(mockFormField.Id).thenReturn(EXAMPLE_FORM_PROPERTY_ID);
		when(mockFormField.Label).thenReturn(EXAMPLE_FORM_PROPERTY_NAME);
		when(mockFormField.DefaultValue).thenReturn(EXAMPLE_FORM_PROPERTY_VALUE);

		FormType mockFormType = mock(typeof(FormType));
		when(mockFormType.Name).thenReturn(EXAMPLE_FORM_PROPERTY_TYPE_NAME);
		when(mockFormField.Type).thenReturn(mockFormType);

		StartFormData mockFormData = mock(typeof(StartFormData));
		when(mockFormData.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		when(mockFormData.ProcessDefinition).thenReturn(definition);

		IList<FormField> mockFormFields = new List<FormField>();
		mockFormFields.Add(mockFormField);
		when(mockFormData.FormFields).thenReturn(mockFormFields);

		return mockFormData;
	  }

	  public static ProcessInstanceWithVariables createMockInstanceWithVariables()
	  {
		return createMockInstanceWithVariables(EXAMPLE_TENANT_ID);
	  }

	  public static ProcessInstanceWithVariables createMockInstanceWithVariables(string tenantId)
	  {
		ProcessInstanceWithVariables mock = mock(typeof(ProcessInstanceWithVariables));

		when(mock.Id).thenReturn(EXAMPLE_PROCESS_INSTANCE_ID);
		when(mock.BusinessKey).thenReturn(EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY);
		when(mock.CaseInstanceId).thenReturn(EXAMPLE_CASE_INSTANCE_ID);
		when(mock.ProcessDefinitionId).thenReturn(EXAMPLE_PROCESS_DEFINITION_ID);
		when(mock.ProcessInstanceId).thenReturn(EXAMPLE_PROCESS_INSTANCE_ID);
		when(mock.Suspended).thenReturn(EXAMPLE_PROCESS_INSTANCE_IS_SUSPENDED);
		when(mock.Ended).thenReturn(EXAMPLE_PROCESS_INSTANCE_IS_ENDED);
		when(mock.TenantId).thenReturn(tenantId);
		when(mock.Variables).thenReturn(createMockSerializedVariables());
		return mock;
	  }


	  public static ProcessInstance createMockInstance()
	  {
		return createMockInstance(EXAMPLE_TENANT_ID);
	  }

	  public static ProcessInstance createMockInstance(string tenantId)
	  {
		ProcessInstance mock = mock(typeof(ProcessInstance));

		when(mock.Id).thenReturn(EXAMPLE_PROCESS_INSTANCE_ID);
		when(mock.BusinessKey).thenReturn(EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY);
		when(mock.CaseInstanceId).thenReturn(EXAMPLE_CASE_INSTANCE_ID);
		when(mock.ProcessDefinitionId).thenReturn(EXAMPLE_PROCESS_DEFINITION_ID);
		when(mock.ProcessInstanceId).thenReturn(EXAMPLE_PROCESS_INSTANCE_ID);
		when(mock.Suspended).thenReturn(EXAMPLE_PROCESS_INSTANCE_IS_SUSPENDED);
		when(mock.Ended).thenReturn(EXAMPLE_PROCESS_INSTANCE_IS_ENDED);
		when(mock.TenantId).thenReturn(tenantId);

		return mock;
	  }

	  public static VariableInstance createMockVariableInstance()
	  {
		return mockVariableInstance().build();
	  }

	  public static MockVariableInstanceBuilder mockVariableInstance()
	  {
		return (new MockVariableInstanceBuilder()).id(EXAMPLE_VARIABLE_INSTANCE_ID).name(EXAMPLE_VARIABLE_INSTANCE_NAME).typedValue(EXAMPLE_PRIMITIVE_VARIABLE_VALUE).processInstanceId(EXAMPLE_VARIABLE_INSTANCE_PROC_INST_ID).executionId(EXAMPLE_VARIABLE_INSTANCE_EXECUTION_ID).caseInstanceId(EXAMPLE_VARIABLE_INSTANCE_CASE_INST_ID).caseExecutionId(EXAMPLE_VARIABLE_INSTANCE_CASE_EXECUTION_ID).taskId(EXAMPLE_VARIABLE_INSTANCE_TASK_ID).activityInstanceId(EXAMPLE_VARIABLE_INSTANCE_ACTIVITY_INSTANCE_ID).tenantId(EXAMPLE_TENANT_ID).errorMessage(null);
	  }

	  public static VariableInstance createMockVariableInstance(TypedValue value)
	  {
		return mockVariableInstance().typedValue(value).build();
	  }

	  public static VariableMap createMockSerializedVariables()
	  {
		VariableMap variables = Variables.createVariables();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue serializedVar = Variables.serializedObjectValue(EXAMPLE_VARIABLE_INSTANCE_SERIALIZED_VALUE).serializationDataFormat(FORMAT_APPLICATION_JSON).objectTypeName(typeof(List<object>).FullName).create();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ObjectValue deserializedVar = new ObjectValueImpl(EXAMPLE_VARIABLE_INSTANCE_DESERIALIZED_VALUE, EXAMPLE_VARIABLE_INSTANCE_SERIALIZED_VALUE, FORMAT_APPLICATION_JSON, typeof(object).FullName, true);
		variables.putValueTyped(EXAMPLE_VARIABLE_INSTANCE_NAME, serializedVar);
		variables.putValueTyped(EXAMPLE_DESERIALIZED_VARIABLE_INSTANCE_NAME, deserializedVar);
		return variables;
	  }

	  public static Execution createMockExecution()
	  {
		return createMockExecution(EXAMPLE_TENANT_ID);
	  }

	  public static Execution createMockExecution(string tenantId)
	  {
		Execution mock = mock(typeof(Execution));

		when(mock.Id).thenReturn(EXAMPLE_EXECUTION_ID);
		when(mock.ProcessInstanceId).thenReturn(EXAMPLE_PROCESS_INSTANCE_ID);
		when(mock.Ended).thenReturn(EXAMPLE_EXECUTION_IS_ENDED);
		when(mock.TenantId).thenReturn(tenantId);

		return mock;
	  }

	  public static EventSubscription createMockEventSubscription()
	  {
		EventSubscription mock = mock(typeof(EventSubscription));

		when(mock.Id).thenReturn(EXAMPLE_EVENT_SUBSCRIPTION_ID);
		when(mock.EventType).thenReturn(EXAMPLE_EVENT_SUBSCRIPTION_TYPE);
		when(mock.EventName).thenReturn(EXAMPLE_EVENT_SUBSCRIPTION_NAME);
		when(mock.ExecutionId).thenReturn(EXAMPLE_EXECUTION_ID);
		when(mock.ProcessInstanceId).thenReturn(EXAMPLE_PROCESS_INSTANCE_ID);
		when(mock.ActivityId).thenReturn(EXAMPLE_ACTIVITY_ID);
		when(mock.Created).thenReturn(DateTimeUtil.parseDate(EXAMPLE_EVENT_SUBSCRIPTION_CREATION_DATE));
		when(mock.TenantId).thenReturn(EXAMPLE_TENANT_ID);

		return mock;
	  }

	  // statistics
	  public static IList<ProcessDefinitionStatistics> createMockProcessDefinitionStatistics()
	  {
		ProcessDefinitionStatistics statistics = mock(typeof(ProcessDefinitionStatistics));
		when(statistics.FailedJobs).thenReturn(EXAMPLE_FAILED_JOBS);
		when(statistics.Instances).thenReturn(EXAMPLE_INSTANCES);
		when(statistics.Id).thenReturn(EXAMPLE_PROCESS_DEFINITION_ID);
		when(statistics.Name).thenReturn(EXAMPLE_PROCESS_DEFINITION_NAME);
		when(statistics.Key).thenReturn(EXAMPLE_PROCESS_DEFINITION_KEY);
		when(statistics.TenantId).thenReturn(EXAMPLE_TENANT_ID);
		when(statistics.VersionTag).thenReturn(EXAMPLE_VERSION_TAG);
		when(statistics.Category).thenReturn(EXAMPLE_PROCESS_DEFINITION_CATEGORY);
		when(statistics.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		when(statistics.DiagramResourceName).thenReturn(EXAMPLE_PROCESS_DEFINITION_DIAGRAM_RESOURCE_NAME);
		when(statistics.ResourceName).thenReturn(EXAMPLE_PROCESS_DEFINITION_RESOURCE_NAME);
		when(statistics.Version).thenReturn(EXAMPLE_PROCESS_DEFINITION_VERSION);
		when(statistics.Description).thenReturn(EXAMPLE_PROCESS_DEFINITION_DESCRIPTION);

		IncidentStatistics incidentStaticits = mock(typeof(IncidentStatistics));
		when(incidentStaticits.IncidentType).thenReturn(EXAMPLE_INCIDENT_TYPE);
		when(incidentStaticits.IncidentCount).thenReturn(EXAMPLE_INCIDENT_COUNT);

		IList<IncidentStatistics> exampleIncidentList = new List<IncidentStatistics>();
		exampleIncidentList.Add(incidentStaticits);
		when(statistics.IncidentStatistics).thenReturn(exampleIncidentList);

		ProcessDefinitionStatistics anotherStatistics = mock(typeof(ProcessDefinitionStatistics));
		when(anotherStatistics.FailedJobs).thenReturn(ANOTHER_EXAMPLE_FAILED_JOBS);
		when(anotherStatistics.Instances).thenReturn(ANOTHER_EXAMPLE_INSTANCES);
		when(anotherStatistics.Id).thenReturn(ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID);
		when(anotherStatistics.Name).thenReturn(EXAMPLE_PROCESS_DEFINITION_NAME);
		when(anotherStatistics.Key).thenReturn(EXAMPLE_PROCESS_DEFINITION_KEY);
		when(anotherStatistics.TenantId).thenReturn(ANOTHER_EXAMPLE_TENANT_ID);
		when(anotherStatistics.VersionTag).thenReturn(ANOTHER_EXAMPLE_VERSION_TAG);

		IncidentStatistics anotherIncidentStaticits = mock(typeof(IncidentStatistics));
		when(anotherIncidentStaticits.IncidentType).thenReturn(ANOTHER_EXAMPLE_INCIDENT_TYPE);
		when(anotherIncidentStaticits.IncidentCount).thenReturn(ANOTHER_EXAMPLE_INCIDENT_COUNT);

		IList<IncidentStatistics> anotherExampleIncidentList = new List<IncidentStatistics>();
		anotherExampleIncidentList.Add(anotherIncidentStaticits);
		when(anotherStatistics.IncidentStatistics).thenReturn(anotherExampleIncidentList);

		IList<ProcessDefinitionStatistics> processDefinitionResults = new List<ProcessDefinitionStatistics>();
		processDefinitionResults.Add(statistics);
		processDefinitionResults.Add(anotherStatistics);

		return processDefinitionResults;
	  }

	  public static IList<ActivityStatistics> createMockActivityStatistics()
	  {
		ActivityStatistics statistics = mock(typeof(ActivityStatistics));
		when(statistics.FailedJobs).thenReturn(EXAMPLE_FAILED_JOBS);
		when(statistics.Instances).thenReturn(EXAMPLE_INSTANCES);
		when(statistics.Id).thenReturn(EXAMPLE_ACTIVITY_ID);

		IncidentStatistics incidentStaticits = mock(typeof(IncidentStatistics));
		when(incidentStaticits.IncidentType).thenReturn(EXAMPLE_INCIDENT_TYPE);
		when(incidentStaticits.IncidentCount).thenReturn(EXAMPLE_INCIDENT_COUNT);

		IList<IncidentStatistics> exampleIncidentList = new List<IncidentStatistics>();
		exampleIncidentList.Add(incidentStaticits);
		when(statistics.IncidentStatistics).thenReturn(exampleIncidentList);

		ActivityStatistics anotherStatistics = mock(typeof(ActivityStatistics));
		when(anotherStatistics.FailedJobs).thenReturn(ANOTHER_EXAMPLE_FAILED_JOBS);
		when(anotherStatistics.Instances).thenReturn(ANOTHER_EXAMPLE_INSTANCES);
		when(anotherStatistics.Id).thenReturn(ANOTHER_EXAMPLE_ACTIVITY_ID);

		IncidentStatistics anotherIncidentStaticits = mock(typeof(IncidentStatistics));
		when(anotherIncidentStaticits.IncidentType).thenReturn(ANOTHER_EXAMPLE_INCIDENT_TYPE);
		when(anotherIncidentStaticits.IncidentCount).thenReturn(ANOTHER_EXAMPLE_INCIDENT_COUNT);

		IList<IncidentStatistics> anotherExampleIncidentList = new List<IncidentStatistics>();
		anotherExampleIncidentList.Add(anotherIncidentStaticits);
		when(anotherStatistics.IncidentStatistics).thenReturn(anotherExampleIncidentList);

		IList<ActivityStatistics> activityResults = new List<ActivityStatistics>();
		activityResults.Add(statistics);
		activityResults.Add(anotherStatistics);

		return activityResults;
	  }

	  // process definition
	  public static IList<ProcessDefinition> createMockDefinitions()
	  {
		IList<ProcessDefinition> mocks = new List<ProcessDefinition>();
		mocks.Add(createMockDefinition());
		return mocks;
	  }

	  public static IList<ProcessDefinition> createMockTwoDefinitions()
	  {
		IList<ProcessDefinition> mocks = new List<ProcessDefinition>();
		mocks.Add(createMockDefinition());
		mocks.Add(createMockAnotherDefinition());
		return mocks;
	  }

	  public static MockDefinitionBuilder mockDefinition()
	  {
		return (new MockDefinitionBuilder()).id(EXAMPLE_PROCESS_DEFINITION_ID).category(EXAMPLE_PROCESS_DEFINITION_CATEGORY).name(EXAMPLE_PROCESS_DEFINITION_NAME).key(EXAMPLE_PROCESS_DEFINITION_KEY).description(EXAMPLE_PROCESS_DEFINITION_DESCRIPTION).versionTag(EXAMPLE_VERSION_TAG).version(EXAMPLE_PROCESS_DEFINITION_VERSION).resource(EXAMPLE_PROCESS_DEFINITION_RESOURCE_NAME).deploymentId(EXAMPLE_DEPLOYMENT_ID).diagram(EXAMPLE_PROCESS_DEFINITION_DIAGRAM_RESOURCE_NAME).suspended(EXAMPLE_PROCESS_DEFINITION_IS_SUSPENDED);
	  }

	  public static ProcessDefinition createMockDefinition()
	  {
		return mockDefinition().build();
	  }

	  public static ProcessDefinition createMockAnotherDefinition()
	  {
		return mockDefinition().id(ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID).build();
	  }

	  // deployments
	  public static IList<Deployment> createMockDeployments()
	  {
		IList<Deployment> mocks = new List<Deployment>();
		mocks.Add(createMockDeployment());
		return mocks;
	  }

	  public static Deployment createMockDeployment()
	  {
		return createMockDeployment(EXAMPLE_TENANT_ID);
	  }

	  public static Deployment createMockDeployment(string tenantId)
	  {
		Deployment mockDeployment = mock(typeof(Deployment));
		when(mockDeployment.Id).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		when(mockDeployment.Name).thenReturn(EXAMPLE_DEPLOYMENT_NAME);
		when(mockDeployment.DeploymentTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_DEPLOYMENT_TIME));
		when(mockDeployment.Source).thenReturn(EXAMPLE_DEPLOYMENT_SOURCE);
		when(mockDeployment.TenantId).thenReturn(tenantId);
		return mockDeployment;
	  }

	  public static DeploymentWithDefinitions createMockDeploymentWithDefinitions()
	  {
		return createMockDeploymentWithDefinitions(EXAMPLE_TENANT_ID);
	  }

	  public static DeploymentWithDefinitions createMockDeploymentWithDefinitions(string tenantId)
	  {
		DeploymentWithDefinitions mockDeployment = mock(typeof(DeploymentWithDefinitions));
		when(mockDeployment.Id).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		when(mockDeployment.Name).thenReturn(EXAMPLE_DEPLOYMENT_NAME);
		when(mockDeployment.DeploymentTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_DEPLOYMENT_TIME));
		when(mockDeployment.Source).thenReturn(EXAMPLE_DEPLOYMENT_SOURCE);
		when(mockDeployment.TenantId).thenReturn(tenantId);
		IList<ProcessDefinition> mockDefinitions = createMockDefinitions();
		when(mockDeployment.DeployedProcessDefinitions).thenReturn(mockDefinitions);

		IList<CaseDefinition> mockCaseDefinitions = createMockCaseDefinitions();
		when(mockDeployment.DeployedCaseDefinitions).thenReturn(mockCaseDefinitions);

		IList<DecisionDefinition> mockDecisionDefinitions = createMockDecisionDefinitions();
		when(mockDeployment.DeployedDecisionDefinitions).thenReturn(mockDecisionDefinitions);

		IList<DecisionRequirementsDefinition> mockDecisionRequirementsDefinitions = createMockDecisionRequirementsDefinitions();
		when(mockDeployment.DeployedDecisionRequirementsDefinitions).thenReturn(mockDecisionRequirementsDefinitions);

		return mockDeployment;
	  }

	  public static Deployment createMockRedeployment()
	  {
		Deployment mockDeployment = mock(typeof(Deployment));
		when(mockDeployment.Id).thenReturn(EXAMPLE_RE_DEPLOYMENT_ID);
		when(mockDeployment.Name).thenReturn(EXAMPLE_DEPLOYMENT_NAME);
		when(mockDeployment.DeploymentTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_DEPLOYMENT_TIME));
		when(mockDeployment.Source).thenReturn(EXAMPLE_DEPLOYMENT_SOURCE);

		return mockDeployment;
	  }

	  // deployment resources
	  public static IList<Resource> createMockDeploymentResources()
	  {
		IList<Resource> mocks = new List<Resource>();
		mocks.Add(createMockDeploymentResource());
		return mocks;
	  }

	  public static Resource createMockDeploymentResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);

		return mockResource;
	  }

	  public static Resource createMockDeploymentSvgResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_SVG_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_SVG_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentPngResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_PNG_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_PNG_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentJpgResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_JPG_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_JPG_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentJpegResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_JPEG_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_JPEG_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentJpeResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_JPE_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_JPE_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentGifResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_GIF_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_GIF_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentTifResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_TIF_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_TIF_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentTiffResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_TIFF_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_TIFF_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentBpmnResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_BPMN_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_BPMN_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentBpmnXmlResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_BPMN_XML_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_BPMN_XML_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentCmmnResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_CMMN_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_CMMN_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentCmmnXmlResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_CMMN_XML_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_CMMN_XML_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentDmnResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_DMN_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_DMN_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentDmnXmlResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_DMN_XML_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_DMN_XML_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentXmlResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_XML_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_XML_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentJsonResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_JSON_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_JSON_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentGroovyResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_GROOVY_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_GROOVY_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentJavaResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_JAVA_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_JAVA_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentJsResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_JS_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_JS_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentPhpResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_PHP_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_PHP_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentPythonResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_PYTHON_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_PYTHON_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentRubyResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_RUBY_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_RUBY_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentHtmlResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_HTML_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_HTML_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentTxtResource()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_TXT_RESOURCE_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_TXT_RESOURCE_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentResourceFilename()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_RESOURCE_FILENAME_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_RESOURCE_FILENAME_PATH + EXAMPLE_DEPLOYMENT_RESOURCE_FILENAME_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  public static Resource createMockDeploymentResourceFilenameBackslash()
	  {
		Resource mockResource = mock(typeof(ResourceEntity));
		when(mockResource.Id).thenReturn(EXAMPLE_DEPLOYMENT_RESOURCE_FILENAME_ID);
		when(mockResource.Name).thenReturn(EXAMPLE_DEPLOYMENT_RESOURCE_FILENAME_PATH_BACKSLASH + EXAMPLE_DEPLOYMENT_RESOURCE_FILENAME_NAME);
		when(mockResource.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		return mockResource;
	  }

	  // user, groups and tenants

	  public static Group createMockGroup()
	  {
		return mockGroup().build();
	  }

	  public static MockGroupBuilder mockGroup()
	  {
		return (new MockGroupBuilder()).id(EXAMPLE_GROUP_ID).name(EXAMPLE_GROUP_NAME).type(EXAMPLE_GROUP_TYPE);
	  }

	  public static Group createMockGroupUpdate()
	  {
		Group mockGroup = mock(typeof(Group));
		when(mockGroup.Id).thenReturn(EXAMPLE_GROUP_ID);
		when(mockGroup.Name).thenReturn(EXAMPLE_GROUP_NAME_UPDATE);

		return mockGroup;
	  }

	  public static IList<Group> createMockGroups()
	  {
		IList<Group> mockGroups = new List<Group>();
		mockGroups.Add(createMockGroup());
		return mockGroups;
	  }

	  public static User createMockUser()
	  {
		return mockUser().build();
	  }

	  public static MockUserBuilder mockUser()
	  {
		return (new MockUserBuilder()).id(EXAMPLE_USER_ID).firstName(EXAMPLE_USER_FIRST_NAME).lastName(EXAMPLE_USER_LAST_NAME).email(EXAMPLE_USER_EMAIL).password(EXAMPLE_USER_PASSWORD);
	  }

	  public static Tenant createMockTenant()
	  {
		Tenant mockTenant = mock(typeof(Tenant));
		when(mockTenant.Id).thenReturn(EXAMPLE_TENANT_ID);
		when(mockTenant.Name).thenReturn(EXAMPLE_TENANT_NAME);
		return mockTenant;
	  }

	  public static Authentication createMockAuthentication()
	  {
		Authentication mockAuthentication = mock(typeof(Authentication));

		when(mockAuthentication.UserId).thenReturn(EXAMPLE_USER_ID);

		return mockAuthentication;
	  }

	  // jobs
	  public static Job createMockJob()
	  {
		return mockJob().tenantId(EXAMPLE_TENANT_ID).build();
	  }

	  public static MockJobBuilder mockJob()
	  {
		return (new MockJobBuilder()).id(EXAMPLE_JOB_ID).processInstanceId(EXAMPLE_PROCESS_INSTANCE_ID).executionId(EXAMPLE_EXECUTION_ID).processDefinitionId(EXAMPLE_PROCESS_DEFINITION_ID).processDefinitionKey(EXAMPLE_PROCESS_DEFINITION_KEY).retries(EXAMPLE_JOB_RETRIES).exceptionMessage(EXAMPLE_JOB_NO_EXCEPTION_MESSAGE).dueDate(DateTimeUtil.parseDate(EXAMPLE_DUE_DATE)).suspended(EXAMPLE_JOB_IS_SUSPENDED.Value).priority(EXAMPLE_JOB_PRIORITY).jobDefinitionId(EXAMPLE_JOB_DEFINITION_ID).createTime(DateTimeUtil.parseDate(EXAMPLE_JOB_CREATE_TIME));
	  }

	  public static IList<Job> createMockJobs()
	  {
		IList<Job> mockList = new List<Job>();
		mockList.Add(createMockJob());
		return mockList;
	  }

	  public static IList<Job> createMockEmptyJobList()
	  {
		return new List<Job>();
	  }

	  public static User createMockUserUpdate()
	  {
		User mockUser = mock(typeof(User));
		when(mockUser.Id).thenReturn(EXAMPLE_USER_ID);
		when(mockUser.FirstName).thenReturn(EXAMPLE_USER_FIRST_NAME_UPDATE);
		when(mockUser.LastName).thenReturn(EXAMPLE_USER_LAST_NAME_UPDATE);
		when(mockUser.Email).thenReturn(EXAMPLE_USER_EMAIL_UPDATE);
		when(mockUser.Password).thenReturn(EXAMPLE_USER_PASSWORD);
		return mockUser;
	  }

	  public static IList<User> createMockUsers()
	  {
		List<User> list = new List<User>();
		list.Add(createMockUser());
		return list;
	  }

	  public static Authorization createMockGlobalAuthorization()
	  {
		Authorization mockAuthorization = mock(typeof(Authorization));

		when(mockAuthorization.Id).thenReturn(EXAMPLE_AUTHORIZATION_ID);
		when(mockAuthorization.AuthorizationType).thenReturn(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL);
		when(mockAuthorization.UserId).thenReturn(org.camunda.bpm.engine.authorization.Authorization_Fields.ANY);

		when(mockAuthorization.ResourceType).thenReturn(EXAMPLE_RESOURCE_TYPE_ID);
		when(mockAuthorization.ResourceId).thenReturn(EXAMPLE_RESOURCE_ID);
		when(mockAuthorization.getPermissions(Permissions.values())).thenReturn(EXAMPLE_GRANT_PERMISSION_VALUES);

		return mockAuthorization;
	  }

	  public static Authorization createMockGrantAuthorization()
	  {
		Authorization mockAuthorization = mock(typeof(Authorization));

		when(mockAuthorization.Id).thenReturn(EXAMPLE_AUTHORIZATION_ID);
		when(mockAuthorization.AuthorizationType).thenReturn(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT);
		when(mockAuthorization.UserId).thenReturn(EXAMPLE_USER_ID);

		when(mockAuthorization.ResourceType).thenReturn(EXAMPLE_RESOURCE_TYPE_ID);
		when(mockAuthorization.ResourceId).thenReturn(EXAMPLE_RESOURCE_ID);
		when(mockAuthorization.getPermissions(Permissions.values())).thenReturn(EXAMPLE_GRANT_PERMISSION_VALUES);

		return mockAuthorization;
	  }

	  public static Authorization createMockRevokeAuthorization()
	  {
		Authorization mockAuthorization = mock(typeof(Authorization));

		when(mockAuthorization.Id).thenReturn(EXAMPLE_AUTHORIZATION_ID);
		when(mockAuthorization.AuthorizationType).thenReturn(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_REVOKE);
		when(mockAuthorization.UserId).thenReturn(EXAMPLE_USER_ID);

		when(mockAuthorization.ResourceType).thenReturn(EXAMPLE_RESOURCE_TYPE_ID);
		when(mockAuthorization.ResourceId).thenReturn(EXAMPLE_RESOURCE_ID);
		when(mockAuthorization.getPermissions(Permissions.values())).thenReturn(EXAMPLE_REVOKE_PERMISSION_VALUES);

		return mockAuthorization;
	  }

	  public static IList<Authorization> createMockAuthorizations()
	  {
		return Arrays.asList(new Authorization[] {createMockGlobalAuthorization(), createMockGrantAuthorization(), createMockRevokeAuthorization()});
	  }

	  public static IList<Authorization> createMockGrantAuthorizations()
	  {
		return Arrays.asList(new Authorization[] {createMockGrantAuthorization()});
	  }

	  public static IList<Authorization> createMockRevokeAuthorizations()
	  {
		return Arrays.asList(new Authorization[]{createMockRevokeAuthorization()});
	  }

	  public static IList<Authorization> createMockGlobalAuthorizations()
	  {
		return Arrays.asList(new Authorization[] {createMockGlobalAuthorization()});
	  }

	  public static DateTime createMockDuedate()
	  {
		DateTime cal = new DateTime();
		cal = new DateTime(DateTime.Now);
		cal.AddDays(3);
		return cal;
	  } // process application

	  public static ProcessApplicationInfo createMockProcessApplicationInfo()
	  {
		ProcessApplicationInfo appInfo = mock(typeof(ProcessApplicationInfo));
		IDictionary<string, string> mockAppProperties = new Dictionary<string, string>();
		string mockServletContextPath = MockProvider.EXAMPLE_PROCESS_APPLICATION_CONTEXT_PATH;
		mockAppProperties[org.camunda.bpm.application.ProcessApplicationInfo_Fields.PROP_SERVLET_CONTEXT_PATH] = mockServletContextPath;
		when(appInfo.Properties).thenReturn(mockAppProperties);
		return appInfo;
	  }

	  // History
	  public static IList<HistoricActivityInstance> createMockHistoricActivityInstances()
	  {
		IList<HistoricActivityInstance> mockList = new List<HistoricActivityInstance>();
		mockList.Add(createMockHistoricActivityInstance());
		return mockList;
	  }

	  public static HistoricActivityInstance createMockHistoricActivityInstance()
	  {
		return createMockHistoricActivityInstance(EXAMPLE_TENANT_ID);
	  }

	  public static HistoricActivityInstance createMockHistoricActivityInstance(string tenantId)
	  {
		HistoricActivityInstance mock = mock(typeof(HistoricActivityInstance));

		when(mock.Id).thenReturn(EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_ID);
		when(mock.ParentActivityInstanceId).thenReturn(EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_PARENT_ACTIVITY_INSTANCE_ID);
		when(mock.ActivityId).thenReturn(EXAMPLE_ACTIVITY_ID);
		when(mock.ActivityName).thenReturn(EXAMPLE_ACTIVITY_NAME);
		when(mock.ActivityType).thenReturn(EXAMPLE_ACTIVITY_TYPE);
		when(mock.ProcessDefinitionKey).thenReturn(EXAMPLE_PROCESS_DEFINITION_KEY);
		when(mock.ProcessDefinitionId).thenReturn(EXAMPLE_PROCESS_DEFINITION_ID);
		when(mock.ProcessInstanceId).thenReturn(EXAMPLE_PROCESS_INSTANCE_ID);
		when(mock.ExecutionId).thenReturn(EXAMPLE_EXECUTION_ID);
		when(mock.TaskId).thenReturn(EXAMPLE_TASK_ID);
		when(mock.CalledProcessInstanceId).thenReturn(EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_CALLED_PROCESS_INSTANCE_ID);
		when(mock.CalledCaseInstanceId).thenReturn(EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_CALLED_CASE_INSTANCE_ID);
		when(mock.Assignee).thenReturn(EXAMPLE_TASK_ASSIGNEE_NAME);
		when(mock.StartTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_START_TIME));
		when(mock.EndTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_END_TIME));
		when(mock.DurationInMillis).thenReturn(EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_DURATION);
		when(mock.Canceled).thenReturn(EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_IS_CANCELED);
		when(mock.CompleteScope).thenReturn(EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_IS_COMPLETE_SCOPE);
		when(mock.TenantId).thenReturn(tenantId);
		when(mock.RemovalTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_REMOVAL_TIME));
		when(mock.RootProcessInstanceId).thenReturn(EXAMPLE_HISTORIC_ACTIVITY_ROOT_PROCESS_INSTANCE_ID);

		return mock;
	  }

	  public static IList<HistoricActivityInstance> createMockRunningHistoricActivityInstances()
	  {
		IList<HistoricActivityInstance> mockList = new List<HistoricActivityInstance>();
		mockList.Add(createMockRunningHistoricActivityInstance());
		return mockList;
	  }

	  public static HistoricActivityInstance createMockRunningHistoricActivityInstance()
	  {
		HistoricActivityInstance mock = mock(typeof(HistoricActivityInstance));

		when(mock.Id).thenReturn(EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_ID);
		when(mock.ParentActivityInstanceId).thenReturn(EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_PARENT_ACTIVITY_INSTANCE_ID);
		when(mock.ActivityId).thenReturn(EXAMPLE_ACTIVITY_ID);
		when(mock.ActivityName).thenReturn(EXAMPLE_ACTIVITY_NAME);
		when(mock.ActivityType).thenReturn(EXAMPLE_ACTIVITY_TYPE);
		when(mock.ProcessDefinitionId).thenReturn(EXAMPLE_PROCESS_DEFINITION_ID);
		when(mock.ProcessInstanceId).thenReturn(EXAMPLE_PROCESS_INSTANCE_ID);
		when(mock.ExecutionId).thenReturn(EXAMPLE_EXECUTION_ID);
		when(mock.TaskId).thenReturn(EXAMPLE_TASK_ID);
		when(mock.CalledProcessInstanceId).thenReturn(EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_CALLED_PROCESS_INSTANCE_ID);
		when(mock.CalledCaseInstanceId).thenReturn(EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_CALLED_CASE_INSTANCE_ID);
		when(mock.Assignee).thenReturn(EXAMPLE_TASK_ASSIGNEE_NAME);
		when(mock.StartTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_START_TIME));
		when(mock.EndTime).thenReturn(null);
		when(mock.DurationInMillis).thenReturn(null);

		return mock;
	  }

	  public static IList<HistoricCaseActivityInstance> createMockHistoricCaseActivityInstances()
	  {
		List<HistoricCaseActivityInstance> mockList = new List<HistoricCaseActivityInstance>();
		mockList.Add(createMockHistoricCaseActivityInstance());
		return mockList;
	  }

	  public static HistoricCaseActivityInstance createMockHistoricCaseActivityInstance()
	  {
		return createMockHistoricCaseActivityInstance(EXAMPLE_TENANT_ID);
	  }

	  public static HistoricCaseActivityInstance createMockHistoricCaseActivityInstance(string tenantId)
	  {
		HistoricCaseActivityInstance mock = mock(typeof(HistoricCaseActivityInstance));

		when(mock.Id).thenReturn(EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_ID);
		when(mock.ParentCaseActivityInstanceId).thenReturn(EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_PARENT_CASE_ACTIVITY_INSTANCE_ID);
		when(mock.CaseActivityId).thenReturn(EXAMPLE_HISTORIC_CASE_ACTIVITY_ID);
		when(mock.CaseActivityName).thenReturn(EXAMPLE_HISTORIC_CASE_ACTIVITY_NAME);
		when(mock.CaseActivityType).thenReturn(EXAMPLE_HISTORIC_CASE_ACTIVITY_TYPE);
		when(mock.CaseDefinitionId).thenReturn(EXAMPLE_CASE_DEFINITION_ID);
		when(mock.CaseInstanceId).thenReturn(EXAMPLE_CASE_INSTANCE_ID);
		when(mock.CaseExecutionId).thenReturn(EXAMPLE_CASE_EXECUTION_ID);
		when(mock.TaskId).thenReturn(EXAMPLE_TASK_ID);
		when(mock.CalledProcessInstanceId).thenReturn(EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_CALLED_PROCESS_INSTANCE_ID);
		when(mock.CalledCaseInstanceId).thenReturn(EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_CALLED_CASE_INSTANCE_ID);
		when(mock.CreateTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_CREATE_TIME));
		when(mock.EndTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_END_TIME));
		when(mock.TenantId).thenReturn(tenantId);
		when(mock.DurationInMillis).thenReturn(EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_DURATION);
		when(mock.Required).thenReturn(EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_REQUIRED);
		when(mock.Available).thenReturn(EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_AVAILABLE);
		when(mock.Enabled).thenReturn(EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_ENABLED);
		when(mock.Disabled).thenReturn(EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_DISABLED);
		when(mock.Active).thenReturn(EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_ACTIVE);
		when(mock.Completed).thenReturn(EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_COMPLETED);
		when(mock.Terminated).thenReturn(EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_TERMINATED);

		return mock;
	  }

	  public static IList<HistoricCaseActivityInstance> createMockRunningHistoricCaseActivityInstances()
	  {
		IList<HistoricCaseActivityInstance> mockList = new List<HistoricCaseActivityInstance>();
		mockList.Add(createMockRunningHistoricCaseActivityInstance());
		return mockList;
	  }

	  public static HistoricCaseActivityInstance createMockRunningHistoricCaseActivityInstance()
	  {
		HistoricCaseActivityInstance mock = createMockHistoricCaseActivityInstance();

		when(mock.EndTime).thenReturn(null);
		when(mock.DurationInMillis).thenReturn(null);
		when(mock.Available).thenReturn(false);
		when(mock.Enabled).thenReturn(false);
		when(mock.Disabled).thenReturn(false);
		when(mock.Active).thenReturn(true);
		when(mock.Completed).thenReturn(false);
		when(mock.Terminated).thenReturn(false);

		return mock;
	  }

	  public static IList<HistoricActivityStatistics> createMockHistoricActivityStatistics()
	  {
		HistoricActivityStatistics statistics = mock(typeof(HistoricActivityStatistics));

		when(statistics.Id).thenReturn(EXAMPLE_ACTIVITY_ID);
		when(statistics.Instances).thenReturn(EXAMPLE_INSTANCES_LONG);
		when(statistics.Canceled).thenReturn(EXAMPLE_CANCELED_LONG);
		when(statistics.Finished).thenReturn(EXAMPLE_FINISHED_LONG);
		when(statistics.CompleteScope).thenReturn(EXAMPLE_COMPLETE_SCOPE_LONG);

		HistoricActivityStatistics anotherStatistics = mock(typeof(HistoricActivityStatistics));

		when(anotherStatistics.Id).thenReturn(ANOTHER_EXAMPLE_ACTIVITY_ID);
		when(anotherStatistics.Instances).thenReturn(ANOTHER_EXAMPLE_INSTANCES_LONG);
		when(anotherStatistics.Canceled).thenReturn(ANOTHER_EXAMPLE_CANCELED_LONG);
		when(anotherStatistics.Finished).thenReturn(ANOTHER_EXAMPLE_FINISHED_LONG);
		when(anotherStatistics.CompleteScope).thenReturn(ANOTHER_EXAMPLE_COMPLETE_SCOPE_LONG);

		IList<HistoricActivityStatistics> activityResults = new List<HistoricActivityStatistics>();
		activityResults.Add(statistics);
		activityResults.Add(anotherStatistics);

		return activityResults;
	  }

	  public static IList<HistoricCaseActivityStatistics> createMockHistoricCaseActivityStatistics()
	  {
		HistoricCaseActivityStatistics statistics = mock(typeof(HistoricCaseActivityStatistics));

		when(statistics.Id).thenReturn(EXAMPLE_ACTIVITY_ID);
		when(statistics.Active).thenReturn(EXAMPLE_ACTIVE_LONG);
		when(statistics.Available).thenReturn(EXAMPLE_AVAILABLE_LONG);
		when(statistics.Completed).thenReturn(EXAMPLE_COMPLETED_LONG);
		when(statistics.Disabled).thenReturn(EXAMPLE_DISABLED_LONG);
		when(statistics.Enabled).thenReturn(EXAMPLE_ENABLED_LONG);
		when(statistics.Terminated).thenReturn(EXAMPLE_TERMINATED_LONG);

		HistoricCaseActivityStatistics anotherStatistics = mock(typeof(HistoricCaseActivityStatistics));

		when(anotherStatistics.Id).thenReturn(ANOTHER_EXAMPLE_ACTIVITY_ID);
		when(anotherStatistics.Active).thenReturn(ANOTHER_EXAMPLE_ACTIVE_LONG);
		when(anotherStatistics.Available).thenReturn(ANOTHER_EXAMPLE_AVAILABLE_LONG);
		when(anotherStatistics.Completed).thenReturn(ANOTHER_EXAMPLE_COMPLETED_LONG);
		when(anotherStatistics.Disabled).thenReturn(ANOTHER_EXAMPLE_DISABLED_LONG);
		when(anotherStatistics.Enabled).thenReturn(ANOTHER_EXAMPLE_ENABLED_LONG);
		when(anotherStatistics.Terminated).thenReturn(ANOTHER_EXAMPLE_TERMINATED_LONG);

		IList<HistoricCaseActivityStatistics> activityResults = new List<HistoricCaseActivityStatistics>();
		activityResults.Add(statistics);
		activityResults.Add(anotherStatistics);

		return activityResults;
	  }

	  public static IList<HistoricProcessInstance> createMockHistoricProcessInstances()
	  {
		IList<HistoricProcessInstance> mockList = new List<HistoricProcessInstance>();
		mockList.Add(createMockHistoricProcessInstance());
		return mockList;
	  }

	  public static HistoricProcessInstance createMockHistoricProcessInstance()
	  {
		return createMockHistoricProcessInstance(EXAMPLE_TENANT_ID);
	  }

	  public static HistoricProcessInstance createMockHistoricProcessInstance(string tenantId)
	  {
		HistoricProcessInstance mock = mock(typeof(HistoricProcessInstance));

		when(mock.Id).thenReturn(EXAMPLE_PROCESS_INSTANCE_ID);
		when(mock.BusinessKey).thenReturn(EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY);
		when(mock.ProcessDefinitionKey).thenReturn(EXAMPLE_PROCESS_DEFINITION_KEY);
		when(mock.ProcessDefinitionName).thenReturn(EXAMPLE_PROCESS_DEFINITION_NAME);
		when(mock.ProcessDefinitionVersion).thenReturn(EXAMPLE_PROCESS_DEFINITION_VERSION);
		when(mock.ProcessDefinitionId).thenReturn(EXAMPLE_PROCESS_DEFINITION_ID);
		when(mock.DeleteReason).thenReturn(EXAMPLE_HISTORIC_PROCESS_INSTANCE_DELETE_REASON);
		when(mock.RemovalTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_PROCESS_INSTANCE_REMOVAL_TIME));
		when(mock.EndTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_PROCESS_INSTANCE_END_TIME));
		when(mock.StartTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_PROCESS_INSTANCE_START_TIME));
		when(mock.DurationInMillis).thenReturn(EXAMPLE_HISTORIC_PROCESS_INSTANCE_DURATION_MILLIS);
		when(mock.StartUserId).thenReturn(EXAMPLE_HISTORIC_PROCESS_INSTANCE_START_USER_ID);
		when(mock.StartActivityId).thenReturn(EXAMPLE_HISTORIC_PROCESS_INSTANCE_START_ACTIVITY_ID);
		when(mock.RootProcessInstanceId).thenReturn(EXAMPLE_HISTORIC_PROCESS_INSTANCE_ROOT_PROCESS_INSTANCE_ID);
		when(mock.SuperProcessInstanceId).thenReturn(EXAMPLE_HISTORIC_PROCESS_INSTANCE_SUPER_PROCESS_INSTANCE_ID);
		when(mock.SuperCaseInstanceId).thenReturn(EXAMPLE_HISTORIC_PROCESS_INSTANCE_SUPER_CASE_INSTANCE_ID);
		when(mock.CaseInstanceId).thenReturn(EXAMPLE_HISTORIC_PROCESS_INSTANCE_CASE_INSTANCE_ID);
		when(mock.TenantId).thenReturn(tenantId);
		when(mock.State).thenReturn(EXAMPLE_HISTORIC_PROCESS_INSTANCE_STATE);

		return mock;
	  }

	  public static IList<HistoricProcessInstance> createMockRunningHistoricProcessInstances()
	  {
		IList<HistoricProcessInstance> mockList = new List<HistoricProcessInstance>();
		mockList.Add(createMockHistoricProcessInstanceUnfinished());
		return mockList;
	  }

	  public static HistoricProcessInstance createMockHistoricProcessInstanceUnfinished()
	  {
		HistoricProcessInstance mock = mock(typeof(HistoricProcessInstance));
		when(mock.Id).thenReturn(EXAMPLE_PROCESS_INSTANCE_ID);
		when(mock.BusinessKey).thenReturn(EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY);
		when(mock.ProcessDefinitionId).thenReturn(EXAMPLE_PROCESS_DEFINITION_ID);
		when(mock.DeleteReason).thenReturn(EXAMPLE_HISTORIC_PROCESS_INSTANCE_DELETE_REASON);
		when(mock.EndTime).thenReturn(null);
		when(mock.StartTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_PROCESS_INSTANCE_START_TIME));
		when(mock.DurationInMillis).thenReturn(EXAMPLE_HISTORIC_PROCESS_INSTANCE_DURATION_MILLIS);
		return mock;
	  }

	  public static IList<DurationReportResult> createMockHistoricProcessInstanceDurationReportByMonth()
	  {
		DurationReportResult mock = mock(typeof(DurationReportResult));
		when(mock.Average).thenReturn(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_AVG);
		when(mock.Minimum).thenReturn(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MIN);
		when(mock.Maximum).thenReturn(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MAX);
		when(mock.Period).thenReturn(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_PERIOD);
		when(mock.PeriodUnit).thenReturn(PeriodUnit.MONTH);

		IList<DurationReportResult> mockList = new List<DurationReportResult>();
		mockList.Add(mock);
		return mockList;
	  }

	  public static IList<DurationReportResult> createMockHistoricProcessInstanceDurationReportByQuarter()
	  {
		DurationReportResult mock = mock(typeof(DurationReportResult));
		when(mock.Average).thenReturn(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_AVG);
		when(mock.Minimum).thenReturn(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MIN);
		when(mock.Maximum).thenReturn(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_MAX);
		when(mock.Period).thenReturn(EXAMPLE_HISTORIC_PROC_INST_DURATION_REPORT_PERIOD);
		when(mock.PeriodUnit).thenReturn(PeriodUnit.QUARTER);

		IList<DurationReportResult> mockList = new List<DurationReportResult>();
		mockList.Add(mock);
		return mockList;
	  }

	  public static IList<HistoricCaseInstance> createMockHistoricCaseInstances()
	  {
		IList<HistoricCaseInstance> mockList = new List<HistoricCaseInstance>();
		mockList.Add(createMockHistoricCaseInstance());
		return mockList;
	  }

	  public static HistoricCaseInstance createMockHistoricCaseInstance()
	  {
		return createMockHistoricCaseInstance(EXAMPLE_TENANT_ID);
	  }

	  public static HistoricCaseInstance createMockHistoricCaseInstance(string tenantId)
	  {
		HistoricCaseInstance mock = mock(typeof(HistoricCaseInstance));

		when(mock.Id).thenReturn(EXAMPLE_CASE_INSTANCE_ID);
		when(mock.BusinessKey).thenReturn(EXAMPLE_CASE_INSTANCE_BUSINESS_KEY);
		when(mock.CaseDefinitionId).thenReturn(EXAMPLE_CASE_DEFINITION_ID);
		when(mock.CaseDefinitionKey).thenReturn(EXAMPLE_CASE_DEFINITION_KEY);
		when(mock.CaseDefinitionName).thenReturn(EXAMPLE_CASE_DEFINITION_NAME);
		when(mock.CreateTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_CASE_INSTANCE_CREATE_TIME));
		when(mock.CloseTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_CASE_INSTANCE_CLOSE_TIME));
		when(mock.DurationInMillis).thenReturn(EXAMPLE_HISTORIC_CASE_INSTANCE_DURATION_MILLIS);
		when(mock.CreateUserId).thenReturn(EXAMPLE_HISTORIC_CASE_INSTANCE_CREATE_USER_ID);
		when(mock.SuperCaseInstanceId).thenReturn(EXAMPLE_HISTORIC_CASE_INSTANCE_SUPER_CASE_INSTANCE_ID);
		when(mock.SuperProcessInstanceId).thenReturn(EXAMPLE_HISTORIC_CASE_INSTANCE_SUPER_PROCESS_INSTANCE_ID);
		when(mock.TenantId).thenReturn(tenantId);
		when(mock.Active).thenReturn(EXAMPLE_HISTORIC_CASE_INSTANCE_IS_ACTIVE);
		when(mock.Completed).thenReturn(EXAMPLE_HISTORIC_CASE_INSTANCE_IS_COMPLETED);
		when(mock.Terminated).thenReturn(EXAMPLE_HISTORIC_CASE_INSTANCE_IS_TERMINATED);
		when(mock.Closed).thenReturn(EXAMPLE_HISTORIC_CASE_INSTANCE_IS_CLOSED);

		return mock;
	  }

	  public static IList<HistoricCaseInstance> createMockRunningHistoricCaseInstances()
	  {
		IList<HistoricCaseInstance> mockList = new List<HistoricCaseInstance>();
		mockList.Add(createMockHistoricCaseInstanceNotClosed());
		return mockList;
	  }

	  public static HistoricCaseInstance createMockHistoricCaseInstanceNotClosed()
	  {
		HistoricCaseInstance mock = createMockHistoricCaseInstance();

		when(mock.CloseTime).thenReturn(null);
		when(mock.DurationInMillis).thenReturn(null);
		when(mock.Active).thenReturn(true);
		when(mock.Completed).thenReturn(false);
		when(mock.Terminated).thenReturn(false);
		when(mock.Closed).thenReturn(false);

		return mock;
	  }

	  public static HistoricVariableInstance createMockHistoricVariableInstance()
	  {
		return mockHistoricVariableInstance(EXAMPLE_TENANT_ID).build();
	  }

	  public static MockHistoricVariableInstanceBuilder mockHistoricVariableInstance()
	  {
		return mockHistoricVariableInstance(EXAMPLE_TENANT_ID);
	  }

	  public static MockHistoricVariableInstanceBuilder mockHistoricVariableInstance(string tenantId)
	  {
		return (new MockHistoricVariableInstanceBuilder()).id(EXAMPLE_VARIABLE_INSTANCE_ID).name(EXAMPLE_VARIABLE_INSTANCE_NAME).typedValue(EXAMPLE_PRIMITIVE_VARIABLE_VALUE).processDefinitionKey(EXAMPLE_VARIABLE_INSTANCE_PROC_DEF_KEY).processDefinitionId(EXAMPLE_VARIABLE_INSTANCE_PROC_DEF_ID).processInstanceId(EXAMPLE_VARIABLE_INSTANCE_PROC_INST_ID).executionId(EXAMPLE_VARIABLE_INSTANCE_EXECUTION_ID).activityInstanceId(EXAMPLE_VARIABLE_INSTANCE_ACTIVITY_INSTANCE_ID).caseDefinitionKey(EXAMPLE_VARIABLE_INSTANCE_CASE_DEF_KEY).caseDefinitionId(EXAMPLE_VARIABLE_INSTANCE_CASE_DEF_ID).caseInstanceId(EXAMPLE_VARIABLE_INSTANCE_CASE_INST_ID).caseExecutionId(EXAMPLE_VARIABLE_INSTANCE_CASE_EXECUTION_ID).taskId(EXAMPLE_VARIABLE_INSTANCE_TASK_ID).tenantId(tenantId).errorMessage(null).createTime(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_VARIABLE_INSTANCE_CREATE_TIME)).removalTime(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_VARIABLE_INSTANCE_REMOVAL_TIME)).rootProcessInstanceId(EXAMPLE_HISTORIC_VARIABLE_INSTANCE_ROOT_PROC_INST_ID);
	  }

	  public static IList<ProcessInstance> createAnotherMockProcessInstanceList()
	  {
		IList<ProcessInstance> mockProcessInstanceList = new List<ProcessInstance>();
		mockProcessInstanceList.Add(createMockInstance());
		mockProcessInstanceList.Add(createAnotherMockInstance());
		return mockProcessInstanceList;
	  }

	  public static ProcessInstance createAnotherMockInstance()
	  {
		ProcessInstance mock = mock(typeof(ProcessInstance));

		when(mock.Id).thenReturn(ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID);
		when(mock.BusinessKey).thenReturn(EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY);
		when(mock.ProcessDefinitionId).thenReturn(EXAMPLE_PROCESS_DEFINITION_ID);
		when(mock.ProcessInstanceId).thenReturn(ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID);
		when(mock.Suspended).thenReturn(EXAMPLE_PROCESS_INSTANCE_IS_SUSPENDED);
		when(mock.Ended).thenReturn(EXAMPLE_PROCESS_INSTANCE_IS_ENDED);

		return mock;
	  }

	  public static ISet<string> createMockSetFromList(string list)
	  {
		return new HashSet<string>(Arrays.asList(list.Split(",", true)));
	  }

	  public static IdentityLink createMockUserAssigneeIdentityLink()
	  {
		IdentityLink identityLink = mock(typeof(IdentityLink));
		when(identityLink.TaskId).thenReturn(EXAMPLE_TASK_ID);
		when(identityLink.Type).thenReturn(IdentityLinkType.ASSIGNEE);
		when(identityLink.UserId).thenReturn(EXAMPLE_TASK_ASSIGNEE_NAME);

		return identityLink;
	  }

	  public static IdentityLink createMockUserOwnerIdentityLink()
	  {
		IdentityLink identityLink = mock(typeof(IdentityLink));
		when(identityLink.TaskId).thenReturn(EXAMPLE_TASK_ID);
		when(identityLink.Type).thenReturn(IdentityLinkType.OWNER);
		when(identityLink.UserId).thenReturn(EXAMPLE_TASK_OWNER);

		return identityLink;
	  }

	  public static IdentityLink createMockCandidateGroupIdentityLink()
	  {
		IdentityLink identityLink = mock(typeof(IdentityLink));
		when(identityLink.TaskId).thenReturn(EXAMPLE_TASK_ID);
		when(identityLink.Type).thenReturn(IdentityLinkType.CANDIDATE);
		when(identityLink.GroupId).thenReturn(EXAMPLE_GROUP_ID);

		return identityLink;
	  }

	  public static IdentityLink createAnotherMockCandidateGroupIdentityLink()
	  {
		IdentityLink identityLink = mock(typeof(IdentityLink));
		when(identityLink.TaskId).thenReturn(EXAMPLE_TASK_ID);
		when(identityLink.Type).thenReturn(IdentityLinkType.CANDIDATE);
		when(identityLink.GroupId).thenReturn(EXAMPLE_GROUP_ID2);

		return identityLink;
	  }

	  // Historic identity link
	  public static HistoricIdentityLinkLog createMockHistoricIdentityLink()
	  {
		return createMockHistoricIdentityLink(EXAMPLE_TENANT_ID);
	  }

	  public static HistoricIdentityLinkLog createMockHistoricIdentityLink(string tenantId)
	  {
		HistoricIdentityLinkLog identityLink = mock(typeof(HistoricIdentityLinkLog));

		when(identityLink.AssignerId).thenReturn(EXAMPLE_HIST_IDENTITY_LINK_ASSIGNER_ID);
		when(identityLink.TenantId).thenReturn(tenantId);
		when(identityLink.GroupId).thenReturn(EXAMPLE_HIST_IDENTITY_LINK_GROUP_ID);
		when(identityLink.TaskId).thenReturn(EXAMPLE_HIST_IDENTITY_LINK_TASK_ID);
		when(identityLink.UserId).thenReturn(EXAMPLE_HIST_IDENTITY_LINK_USER_ID);
		when(identityLink.GroupId).thenReturn(EXAMPLE_HIST_IDENTITY_LINK_GROUP_ID);
		when(identityLink.Type).thenReturn(EXAMPLE_HIST_IDENTITY_LINK_TYPE);
		when(identityLink.OperationType).thenReturn(EXAMPLE_HIST_IDENTITY_LINK_OPERATION_TYPE);
		when(identityLink.ProcessDefinitionId).thenReturn(EXAMPLE_HIST_IDENTITY_LINK_PROC_DEFINITION_ID);
		when(identityLink.ProcessDefinitionKey).thenReturn(EXAMPLE_HIST_IDENTITY_LINK_PROC_DEFINITION_KEY);
		when(identityLink.Time).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HIST_IDENTITY_LINK_TIME));
		when(identityLink.RemovalTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HIST_IDENTITY_LINK_REMOVAL_TIME));
		when(identityLink.RootProcessInstanceId).thenReturn(EXAMPLE_HIST_IDENTITY_LINK_ROOT_PROC_INST_ID);

		return identityLink;
	  }

	  public static IList<HistoricIdentityLinkLog> createMockHistoricIdentityLinks()
	  {
		IList<HistoricIdentityLinkLog> entries = new List<HistoricIdentityLinkLog>();
		entries.Add(createMockHistoricIdentityLink());
		return entries;
	  }

	  // job definition
	  public static IList<JobDefinition> createMockJobDefinitions()
	  {
		IList<JobDefinition> mocks = new List<JobDefinition>();
		mocks.Add(createMockJobDefinition());
		return mocks;
	  }

	  public static JobDefinition createMockJobDefinition()
	  {
		return mockJobDefinition().build();
	  }

	  public static MockJobDefinitionBuilder mockJobDefinition()
	  {
		return (new MockJobDefinitionBuilder()).id(EXAMPLE_JOB_DEFINITION_ID).activityId(EXAMPLE_ACTIVITY_ID).jobConfiguration(EXAMPLE_JOB_CONFIG).jobType(EXAMPLE_JOB_TYPE).jobPriority(EXAMPLE_JOB_DEFINITION_PRIORITY).suspended(EXAMPLE_JOB_DEFINITION_IS_SUSPENDED).processDefinitionId(EXAMPLE_PROCESS_DEFINITION_ID).processDefinitionKey(EXAMPLE_PROCESS_DEFINITION_KEY);
	  }

	  public static IList<UserOperationLogEntry> createUserOperationLogEntries()
	  {
		IList<UserOperationLogEntry> entries = new List<UserOperationLogEntry>();
		entries.Add(createUserOperationLogEntry());
		return entries;
	  }

	  private static UserOperationLogEntry createUserOperationLogEntry()
	  {
		UserOperationLogEntry entry = mock(typeof(UserOperationLogEntry));
		when(entry.Id).thenReturn(EXAMPLE_USER_OPERATION_LOG_ID);
		when(entry.DeploymentId).thenReturn(EXAMPLE_DEPLOYMENT_ID);
		when(entry.ProcessDefinitionId).thenReturn(EXAMPLE_PROCESS_DEFINITION_ID);
		when(entry.ProcessDefinitionKey).thenReturn(EXAMPLE_PROCESS_DEFINITION_KEY);
		when(entry.ProcessInstanceId).thenReturn(EXAMPLE_PROCESS_INSTANCE_ID);
		when(entry.ExecutionId).thenReturn(EXAMPLE_EXECUTION_ID);
		when(entry.CaseDefinitionId).thenReturn(EXAMPLE_CASE_DEFINITION_ID);
		when(entry.CaseInstanceId).thenReturn(EXAMPLE_CASE_INSTANCE_ID);
		when(entry.CaseExecutionId).thenReturn(EXAMPLE_CASE_EXECUTION_ID);
		when(entry.TaskId).thenReturn(EXAMPLE_TASK_ID);
		when(entry.JobId).thenReturn(EXAMPLE_JOB_ID);
		when(entry.JobDefinitionId).thenReturn(EXAMPLE_JOB_DEFINITION_ID);
		when(entry.BatchId).thenReturn(EXAMPLE_BATCH_ID);
		when(entry.UserId).thenReturn(EXAMPLE_USER_ID);
		when(entry.Timestamp).thenReturn(DateTimeUtil.parseDate(EXAMPLE_USER_OPERATION_TIMESTAMP));
		when(entry.OperationId).thenReturn(EXAMPLE_USER_OPERATION_ID);
		when(entry.OperationType).thenReturn(EXAMPLE_USER_OPERATION_TYPE);
		when(entry.EntityType).thenReturn(EXAMPLE_USER_OPERATION_ENTITY);
		when(entry.Property).thenReturn(EXAMPLE_USER_OPERATION_PROPERTY);
		when(entry.OrgValue).thenReturn(EXAMPLE_USER_OPERATION_ORG_VALUE);
		when(entry.NewValue).thenReturn(EXAMPLE_USER_OPERATION_NEW_VALUE);
		return entry;
	  }

	  // historic detail ////////////////////

	  public static HistoricVariableUpdate createMockHistoricVariableUpdate()
	  {
		return mockHistoricVariableUpdate(EXAMPLE_TENANT_ID).build();
	  }

	  public static MockHistoricVariableUpdateBuilder mockHistoricVariableUpdate()
	  {
		return mockHistoricVariableUpdate(EXAMPLE_TENANT_ID);
	  }

	  public static MockHistoricVariableUpdateBuilder mockHistoricVariableUpdate(string tenantId)
	  {
		return (new MockHistoricVariableUpdateBuilder()).id(EXAMPLE_HISTORIC_VAR_UPDATE_ID).processDefinitionKey(EXAMPLE_HISTORIC_VAR_UPDATE_PROC_DEF_KEY).processDefinitionId(EXAMPLE_HISTORIC_VAR_UPDATE_PROC_DEF_ID).processInstanceId(EXAMPLE_HISTORIC_VAR_UPDATE_PROC_INST_ID).activityInstanceId(EXAMPLE_HISTORIC_VAR_UPDATE_ACT_INST_ID).executionId(EXAMPLE_HISTORIC_VAR_UPDATE_EXEC_ID).taskId(EXAMPLE_HISTORIC_VAR_UPDATE_TASK_ID).time(EXAMPLE_HISTORIC_VAR_UPDATE_TIME).name(EXAMPLE_HISTORIC_VAR_UPDATE_NAME).variableInstanceId(EXAMPLE_HISTORIC_VAR_UPDATE_VAR_INST_ID).typedValue(EXAMPLE_PRIMITIVE_VARIABLE_VALUE).revision(EXAMPLE_HISTORIC_VAR_UPDATE_REVISION).errorMessage(null).caseDefinitionKey(EXAMPLE_HISTORIC_VAR_UPDATE_CASE_DEF_KEY).caseDefinitionId(EXAMPLE_HISTORIC_VAR_UPDATE_CASE_DEF_ID).caseInstanceId(EXAMPLE_HISTORIC_VAR_UPDATE_CASE_INST_ID).caseExecutionId(EXAMPLE_HISTORIC_VAR_UPDATE_CASE_EXEC_ID).tenantId(tenantId);
	  }

	  public static HistoricFormField createMockHistoricFormField()
	  {
		return createMockHistoricFormField(EXAMPLE_TENANT_ID);
	  }

	  public static HistoricFormField createMockHistoricFormField(string tenantId)
	  {
		HistoricFormField historicFromField = mock(typeof(HistoricFormField));

		when(historicFromField.Id).thenReturn(EXAMPLE_HISTORIC_FORM_FIELD_ID);
		when(historicFromField.ProcessDefinitionKey).thenReturn(EXAMPLE_HISTORIC_FORM_FIELD_PROC_DEF_KEY);
		when(historicFromField.ProcessDefinitionId).thenReturn(EXAMPLE_HISTORIC_FORM_FIELD_PROC_DEF_ID);
		when(historicFromField.ProcessInstanceId).thenReturn(EXAMPLE_HISTORIC_FORM_FIELD_PROC_INST_ID);
		when(historicFromField.ActivityInstanceId).thenReturn(EXAMPLE_HISTORIC_FORM_FIELD_ACT_INST_ID);
		when(historicFromField.ExecutionId).thenReturn(EXAMPLE_HISTORIC_FORM_FIELD_EXEC_ID);
		when(historicFromField.TaskId).thenReturn(EXAMPLE_HISTORIC_FORM_FIELD_TASK_ID);
		when(historicFromField.Time).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_FORM_FIELD_TIME));
		when(historicFromField.RemovalTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_FORM_FIELD_TIME));
		when(historicFromField.FieldId).thenReturn(EXAMPLE_HISTORIC_FORM_FIELD_FIELD_ID);
		when(historicFromField.FieldValue).thenReturn(EXAMPLE_HISTORIC_FORM_FIELD_VALUE);
		when(historicFromField.CaseDefinitionKey).thenReturn(EXAMPLE_HISTORIC_FORM_FIELD_CASE_DEF_KEY);
		when(historicFromField.CaseDefinitionId).thenReturn(EXAMPLE_HISTORIC_FORM_FIELD_CASE_DEF_ID);
		when(historicFromField.CaseInstanceId).thenReturn(EXAMPLE_HISTORIC_FORM_FIELD_CASE_INST_ID);
		when(historicFromField.CaseExecutionId).thenReturn(EXAMPLE_HISTORIC_FORM_FIELD_CASE_EXEC_ID);
		when(historicFromField.TenantId).thenReturn(tenantId);
		when(historicFromField.UserOperationId).thenReturn(EXAMPLE_HISTORIC_FORM_FIELD_OPERATION_ID);
		when(historicFromField.RootProcessInstanceId).thenReturn(EXAMPLE_HISTORIC_FORM_ROOT_PROCESS_INSTANCE_ID);

		return historicFromField;
	  }

	  public static IList<HistoricFormField> createMockHistoricFormFields()
	  {
		IList<HistoricFormField> entries = new List<HistoricFormField>();
		entries.Add(createMockHistoricFormField());
		return entries;
	  }

	  public static IList<HistoricDetail> createMockHistoricDetails()
	  {
		return createMockHistoricDetails(EXAMPLE_TENANT_ID);
	  }

	  public static IList<HistoricDetail> createMockHistoricDetails(string tenantId)
	  {
		IList<HistoricDetail> entries = new List<HistoricDetail>();
		entries.Add(mockHistoricVariableUpdate(tenantId).build());
		entries.Add(createMockHistoricFormField(tenantId));
		return entries;
	  }

	  public static HistoricTaskInstance createMockHistoricTaskInstance()
	  {
		return createMockHistoricTaskInstance(EXAMPLE_TENANT_ID);
	  }

	  public static HistoricTaskInstance createMockHistoricTaskInstance(string tenantId)
	  {
		HistoricTaskInstance taskInstance = mock(typeof(HistoricTaskInstance));

		when(taskInstance.Id).thenReturn(EXAMPLE_HISTORIC_TASK_INST_ID);
		when(taskInstance.ProcessInstanceId).thenReturn(EXAMPLE_HISTORIC_TASK_INST_PROC_INST_ID);
		when(taskInstance.ActivityInstanceId).thenReturn(EXAMPLE_HISTORIC_TASK_INST_ACT_INST_ID);
		when(taskInstance.ExecutionId).thenReturn(EXAMPLE_HISTORIC_TASK_INST_EXEC_ID);
		when(taskInstance.ProcessDefinitionId).thenReturn(EXAMPLE_HISTORIC_TASK_INST_PROC_DEF_ID);
		when(taskInstance.ProcessDefinitionKey).thenReturn(EXAMPLE_HISTORIC_TASK_INST_PROC_DEF_KEY);
		when(taskInstance.Name).thenReturn(EXAMPLE_HISTORIC_TASK_INST_NAME);
		when(taskInstance.Description).thenReturn(EXAMPLE_HISTORIC_TASK_INST_DESCRIPTION);
		when(taskInstance.DeleteReason).thenReturn(EXAMPLE_HISTORIC_TASK_INST_DELETE_REASON);
		when(taskInstance.Owner).thenReturn(EXAMPLE_HISTORIC_TASK_INST_OWNER);
		when(taskInstance.Assignee).thenReturn(EXAMPLE_HISTORIC_TASK_INST_ASSIGNEE);
		when(taskInstance.StartTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_TASK_INST_START_TIME));
		when(taskInstance.EndTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_TASK_INST_END_TIME));
		when(taskInstance.DurationInMillis).thenReturn(EXAMPLE_HISTORIC_TASK_INST_DURATION);
		when(taskInstance.TaskDefinitionKey).thenReturn(EXAMPLE_HISTORIC_TASK_INST_DEF_KEY);
		when(taskInstance.Priority).thenReturn(EXAMPLE_HISTORIC_TASK_INST_PRIORITY);
		when(taskInstance.DueDate).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_TASK_INST_DUE_DATE));
		when(taskInstance.FollowUpDate).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_TASK_INST_FOLLOW_UP_DATE));
		when(taskInstance.ParentTaskId).thenReturn(EXAMPLE_HISTORIC_TASK_INST_PARENT_TASK_ID);
		when(taskInstance.CaseDefinitionKey).thenReturn(EXAMPLE_HISTORIC_TASK_INST_CASE_DEF_KEY);
		when(taskInstance.CaseDefinitionId).thenReturn(EXAMPLE_HISTORIC_TASK_INST_CASE_DEF_ID);
		when(taskInstance.CaseInstanceId).thenReturn(EXAMPLE_HISTORIC_TASK_INST_CASE_INST_ID);
		when(taskInstance.CaseExecutionId).thenReturn(EXAMPLE_HISTORIC_TASK_INST_CASE_EXEC_ID);
		when(taskInstance.TenantId).thenReturn(tenantId);
		when(taskInstance.RemovalTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_TASK_INST_REMOVAL_TIME));
		when(taskInstance.RootProcessInstanceId).thenReturn(EXAMPLE_HISTORIC_TASK_INST_ROOT_PROC_INST_ID);

		return taskInstance;
	  }

	  public static IList<HistoricTaskInstance> createMockHistoricTaskInstances()
	  {
		IList<HistoricTaskInstance> entries = new List<HistoricTaskInstance>();
		entries.Add(createMockHistoricTaskInstance());
		return entries;
	  }

	  // Incident ///////////////////////////////////////

	  public static Incident createMockIncident()
	  {
		return createMockIncident(EXAMPLE_TENANT_ID);
	  }

	  public static Incident createMockIncident(string tenantId)
	  {
		Incident incident = mock(typeof(Incident));

		when(incident.Id).thenReturn(EXAMPLE_INCIDENT_ID);
		when(incident.IncidentTimestamp).thenReturn(DateTimeUtil.parseDate(EXAMPLE_INCIDENT_TIMESTAMP));
		when(incident.IncidentType).thenReturn(EXAMPLE_INCIDENT_TYPE);
		when(incident.ExecutionId).thenReturn(EXAMPLE_INCIDENT_EXECUTION_ID);
		when(incident.ActivityId).thenReturn(EXAMPLE_INCIDENT_ACTIVITY_ID);
		when(incident.ProcessInstanceId).thenReturn(EXAMPLE_INCIDENT_PROC_INST_ID);
		when(incident.ProcessDefinitionId).thenReturn(EXAMPLE_INCIDENT_PROC_DEF_ID);
		when(incident.CauseIncidentId).thenReturn(EXAMPLE_INCIDENT_CAUSE_INCIDENT_ID);
		when(incident.RootCauseIncidentId).thenReturn(EXAMPLE_INCIDENT_ROOT_CAUSE_INCIDENT_ID);
		when(incident.Configuration).thenReturn(EXAMPLE_INCIDENT_CONFIGURATION);
		when(incident.IncidentMessage).thenReturn(EXAMPLE_INCIDENT_MESSAGE);
		when(incident.TenantId).thenReturn(tenantId);
		when(incident.JobDefinitionId).thenReturn(EXAMPLE_JOB_DEFINITION_ID);

		return incident;
	  }

	  public static IList<Incident> createMockIncidents()
	  {
		IList<Incident> entries = new List<Incident>();
		entries.Add(createMockIncident());
		return entries;
	  }

	  // Historic Incident ///////////////////////////////////////
	  public static HistoricIncident createMockHistoricIncident()
	  {
		return createMockHistoricIncident(EXAMPLE_TENANT_ID);
	  }

	  public static HistoricIncident createMockHistoricIncident(string tenantId)
	  {
		HistoricIncident incident = mock(typeof(HistoricIncident));

		when(incident.Id).thenReturn(EXAMPLE_HIST_INCIDENT_ID);
		when(incident.CreateTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HIST_INCIDENT_CREATE_TIME));
		when(incident.EndTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HIST_INCIDENT_END_TIME));
		when(incident.IncidentType).thenReturn(EXAMPLE_HIST_INCIDENT_TYPE);
		when(incident.ExecutionId).thenReturn(EXAMPLE_HIST_INCIDENT_EXECUTION_ID);
		when(incident.ActivityId).thenReturn(EXAMPLE_HIST_INCIDENT_ACTIVITY_ID);
		when(incident.ProcessInstanceId).thenReturn(EXAMPLE_HIST_INCIDENT_PROC_INST_ID);
		when(incident.ProcessDefinitionId).thenReturn(EXAMPLE_HIST_INCIDENT_PROC_DEF_ID);
		when(incident.ProcessDefinitionKey).thenReturn(EXAMPLE_HIST_INCIDENT_PROC_DEF_KEY);
		when(incident.CauseIncidentId).thenReturn(EXAMPLE_HIST_INCIDENT_CAUSE_INCIDENT_ID);
		when(incident.RootCauseIncidentId).thenReturn(EXAMPLE_HIST_INCIDENT_ROOT_CAUSE_INCIDENT_ID);
		when(incident.Configuration).thenReturn(EXAMPLE_HIST_INCIDENT_CONFIGURATION);
		when(incident.IncidentMessage).thenReturn(EXAMPLE_HIST_INCIDENT_MESSAGE);
		when(incident.Open).thenReturn(EXAMPLE_HIST_INCIDENT_STATE_OPEN);
		when(incident.Deleted).thenReturn(EXAMPLE_HIST_INCIDENT_STATE_DELETED);
		when(incident.Resolved).thenReturn(EXAMPLE_HIST_INCIDENT_STATE_RESOLVED);
		when(incident.TenantId).thenReturn(tenantId);
		when(incident.JobDefinitionId).thenReturn(EXAMPLE_JOB_DEFINITION_ID);
		when(incident.RemovalTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HIST_INCIDENT_REMOVAL_TIME));
		when(incident.RootProcessInstanceId).thenReturn(EXAMPLE_HIST_INCIDENT_ROOT_PROC_INST_ID);

		return incident;
	  }

	  public static IList<HistoricIncident> createMockHistoricIncidents()
	  {
		IList<HistoricIncident> entries = new List<HistoricIncident>();
		entries.Add(createMockHistoricIncident());
		return entries;
	  }

	  // case definition
	  public static IList<CaseDefinition> createMockCaseDefinitions()
	  {
		IList<CaseDefinition> mocks = new List<CaseDefinition>();
		mocks.Add(createMockCaseDefinition());
		return mocks;
	  }

	  public static IList<CaseDefinition> createMockTwoCaseDefinitions()
	  {
		IList<CaseDefinition> mocks = new List<CaseDefinition>();
		mocks.Add(createMockCaseDefinition());
		mocks.Add(createAnotherMockCaseDefinition());
		return mocks;
	  }

	  public static MockCaseDefinitionBuilder mockCaseDefinition()
	  {
		return (new MockCaseDefinitionBuilder()).id(EXAMPLE_CASE_DEFINITION_ID).category(EXAMPLE_CASE_DEFINITION_CATEGORY).name(EXAMPLE_CASE_DEFINITION_NAME).key(EXAMPLE_CASE_DEFINITION_KEY).version(EXAMPLE_CASE_DEFINITION_VERSION).resource(EXAMPLE_CASE_DEFINITION_RESOURCE_NAME).diagram(EXAMPLE_CASE_DEFINITION_DIAGRAM_RESOURCE_NAME).deploymentId(EXAMPLE_DEPLOYMENT_ID);
	  }

	  public static CaseDefinition createMockCaseDefinition()
	  {
		return mockCaseDefinition().build();
	  }

	  public static CaseDefinition createAnotherMockCaseDefinition()
	  {
		return mockCaseDefinition().id(ANOTHER_EXAMPLE_CASE_DEFINITION_ID).tenantId(ANOTHER_EXAMPLE_TENANT_ID).build();
	  }

	  // case instance
	  public static IList<CaseInstance> createMockCaseInstances()
	  {
		IList<CaseInstance> mocks = new List<CaseInstance>();
		mocks.Add(createMockCaseInstance());
		return mocks;
	  }

	  public static CaseInstance createMockCaseInstance()
	  {
		return createMockCaseInstance(EXAMPLE_TENANT_ID);
	  }

	  public static CaseInstance createMockCaseInstance(string tenantId)
	  {
		CaseInstance mock = mock(typeof(CaseInstance));

		when(mock.Id).thenReturn(EXAMPLE_CASE_INSTANCE_ID);
		when(mock.BusinessKey).thenReturn(EXAMPLE_CASE_INSTANCE_BUSINESS_KEY);
		when(mock.CaseDefinitionId).thenReturn(EXAMPLE_CASE_INSTANCE_CASE_DEFINITION_ID);
		when(mock.TenantId).thenReturn(tenantId);
		when(mock.Active).thenReturn(EXAMPLE_CASE_INSTANCE_IS_ACTIVE);
		when(mock.Completed).thenReturn(EXAMPLE_CASE_INSTANCE_IS_COMPLETED);
		when(mock.Terminated).thenReturn(EXAMPLE_CASE_INSTANCE_IS_TERMINATED);

		return mock;
	  }

	  // case execution
	  public static IList<CaseExecution> createMockCaseExecutions()
	  {
		IList<CaseExecution> mocks = new List<CaseExecution>();
		mocks.Add(createMockCaseExecution());
		return mocks;
	  }

	  public static CaseExecution createMockCaseExecution()
	  {
		return createMockCaseExecution(EXAMPLE_TENANT_ID);
	  }

	  public static CaseExecution createMockCaseExecution(string tenantId)
	  {
		CaseExecution mock = mock(typeof(CaseExecution));

		when(mock.Id).thenReturn(EXAMPLE_CASE_EXECUTION_ID);
		when(mock.CaseInstanceId).thenReturn(EXAMPLE_CASE_EXECUTION_CASE_INSTANCE_ID);
		when(mock.ParentId).thenReturn(EXAMPLE_CASE_EXECUTION_PARENT_ID);
		when(mock.CaseDefinitionId).thenReturn(EXAMPLE_CASE_EXECUTION_CASE_DEFINITION_ID);
		when(mock.ActivityId).thenReturn(EXAMPLE_CASE_EXECUTION_ACTIVITY_ID);
		when(mock.ActivityName).thenReturn(EXAMPLE_CASE_EXECUTION_ACTIVITY_NAME);
		when(mock.ActivityType).thenReturn(EXAMPLE_CASE_EXECUTION_ACTIVITY_TYPE);
		when(mock.ActivityDescription).thenReturn(EXAMPLE_CASE_EXECUTION_ACTIVITY_DESCRIPTION);
		when(mock.TenantId).thenReturn(tenantId);
		when(mock.Required).thenReturn(EXAMPLE_CASE_EXECUTION_IS_REQUIRED);
		when(mock.Active).thenReturn(EXAMPLE_CASE_EXECUTION_IS_ACTIVE);
		when(mock.Enabled).thenReturn(EXAMPLE_CASE_EXECUTION_IS_ENABLED);
		when(mock.Disabled).thenReturn(EXAMPLE_CASE_EXECUTION_IS_DISABLED);

		return mock;
	  }

	  public static VariableMap createMockFormVariables()
	  {
		VariableMap mock = Variables.createVariables();
		mock.putValueTyped(EXAMPLE_VARIABLE_INSTANCE_NAME, EXAMPLE_PRIMITIVE_VARIABLE_VALUE);
		return mock;
	  }

	  public static IList<Filter> createMockFilters()
	  {
		IList<Filter> mocks = new List<Filter>();
		mocks.Add(createMockFilter(EXAMPLE_FILTER_ID));
		mocks.Add(createMockFilter(ANOTHER_EXAMPLE_FILTER_ID));
		return mocks;
	  }

	  public static Filter createMockFilter()
	  {
		return createMockFilter(EXAMPLE_FILTER_ID);
	  }

	  public static Filter createMockFilter(string id)
	  {
		return createMockFilter(id, EXAMPLE_FILTER_QUERY);
	  }

	  public static Filter createMockFilter<T1>(string id, Query<T1> query)
	  {
		Filter mock = mockFilter().id(id).resourceType(EXAMPLE_FILTER_RESOURCE_TYPE).name(EXAMPLE_FILTER_NAME).owner(EXAMPLE_FILTER_OWNER).query(query).properties(EXAMPLE_FILTER_PROPERTIES).build();

		doThrow(new NotValidException("Name must not be null")).when(mock).Name = null;
		doThrow(new NotValidException("Name must not be empty")).when(mock).Name = "";
		doThrow(new NotValidException("Query must not be null")).when(mock).Query = null;

		return mock;
	  }

	  public static MockFilterBuilder mockFilter()
	  {
		return (new MockFilterBuilder()).id(EXAMPLE_FILTER_ID).resourceType(EXAMPLE_FILTER_RESOURCE_TYPE).name(EXAMPLE_FILTER_NAME).owner(EXAMPLE_FILTER_OWNER).query(EXAMPLE_FILTER_QUERY).properties(EXAMPLE_FILTER_PROPERTIES);
	  }

	  public static FilterQuery createMockFilterQuery()
	  {
		IList<Filter> mockFilters = createMockFilters();

		FilterQuery query = mock(typeof(FilterQuery));

		when(query.list()).thenReturn(mockFilters);
		when(query.count()).thenReturn((long) mockFilters.Count);
		when(query.filterId(anyString())).thenReturn(query);
		when(query.singleResult()).thenReturn(mockFilters[0]);

		FilterQuery nonExistingQuery = mock(typeof(FilterQuery));
		when(query.filterId(NON_EXISTING_ID)).thenReturn(nonExistingQuery);
		when(nonExistingQuery.singleResult()).thenReturn(null);

		return query;

	  }

	  public static MetricsQuery createMockMeterQuery()
	  {

		MetricsQuery query = mock(typeof(MetricsQuery));

		when(query.name(anyString())).thenReturn(query);
		when(query.reporter(any(typeof(string)))).thenReturn(query);
		when(query.limit(any(typeof(Integer)))).thenReturn(query);
		when(query.offset(any(typeof(Integer)))).thenReturn(query);
		when(query.startDate(any(typeof(DateTime)))).thenReturn(query);
		when(query.endDate(any(typeof(DateTime)))).thenReturn(query);

		return query;

	  }

	  public static IList<MetricIntervalValue> createMockMetricIntervalResult()
	  {
		IList<MetricIntervalValue> metrics = new List<MetricIntervalValue>();

		MetricIntervalEntity entity1 = new MetricIntervalEntity(new DateTime(15 * 60 * 1000 * 1), EXAMPLE_METRICS_NAME, EXAMPLE_METRICS_REPORTER);
		entity1.Value = 21;

		MetricIntervalEntity entity2 = new MetricIntervalEntity(new DateTime(15 * 60 * 1000 * 2), EXAMPLE_METRICS_NAME, EXAMPLE_METRICS_REPORTER);
		entity2.Value = 22;

		MetricIntervalEntity entity3 = new MetricIntervalEntity(new DateTime(15 * 60 * 1000 * 3), EXAMPLE_METRICS_NAME, EXAMPLE_METRICS_REPORTER);
		entity3.Value = 23;

		metrics.Add(entity3);
		metrics.Add(entity2);
		metrics.Add(entity1);

		return metrics;
	  }

	  // decision definition
	  public static IList<DecisionDefinition> createMockDecisionDefinitions()
	  {
		IList<DecisionDefinition> mocks = new List<DecisionDefinition>();
		mocks.Add(createMockDecisionDefinition());
		return mocks;
	  }

	  public static IList<DecisionDefinition> createMockTwoDecisionDefinitions()
	  {
		IList<DecisionDefinition> mocks = new List<DecisionDefinition>();
		mocks.Add(createMockDecisionDefinition());
		mocks.Add(createAnotherMockDecisionDefinition());
		return mocks;
	  }

	  public static MockDecisionDefinitionBuilder mockDecisionDefinition()
	  {
		MockDecisionDefinitionBuilder builder = new MockDecisionDefinitionBuilder();

		return builder.id(EXAMPLE_DECISION_DEFINITION_ID).category(EXAMPLE_DECISION_DEFINITION_CATEGORY).name(EXAMPLE_DECISION_DEFINITION_NAME).key(EXAMPLE_DECISION_DEFINITION_KEY).version(EXAMPLE_DECISION_DEFINITION_VERSION).resource(EXAMPLE_DECISION_DEFINITION_RESOURCE_NAME).diagram(EXAMPLE_DECISION_DEFINITION_DIAGRAM_RESOURCE_NAME).deploymentId(EXAMPLE_DEPLOYMENT_ID).decisionRequirementsDefinitionId(EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID).decisionRequirementsDefinitionKey(EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY);
	  }

	  public static DecisionDefinition createMockDecisionDefinition()
	  {
		return mockDecisionDefinition().build();
	  }

	  public static DecisionDefinition createAnotherMockDecisionDefinition()
	  {
		return mockDecisionDefinition().id(ANOTHER_EXAMPLE_DECISION_DEFINITION_ID).tenantId(ANOTHER_EXAMPLE_TENANT_ID).build();
	  }

	  // decision requirements definition
	  public static MockDecisionRequirementsDefinitionBuilder mockDecisionRequirementsDefinition()
	  {
		MockDecisionRequirementsDefinitionBuilder builder = new MockDecisionRequirementsDefinitionBuilder();

		return builder.id(EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID).category(EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_CATEGORY).name(EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_NAME).key(EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY).version(EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_VERSION).resource(EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_RESOURCE_NAME).diagram(EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_DIAGRAM_RESOURCE_NAME).deploymentId(EXAMPLE_DEPLOYMENT_ID);
	  }

	  public static DecisionRequirementsDefinition createMockDecisionRequirementsDefinition()
	  {
		return mockDecisionRequirementsDefinition().build();
	  }

	  public static DecisionRequirementsDefinition createAnotherMockDecisionRequirementsDefinition()
	  {
		return mockDecisionRequirementsDefinition().id(ANOTHER_EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID).tenantId(ANOTHER_EXAMPLE_TENANT_ID).build();
	  }

	  public static IList<DecisionRequirementsDefinition> createMockDecisionRequirementsDefinitions()
	  {
		IList<DecisionRequirementsDefinition> mocks = new List<DecisionRequirementsDefinition>();
		mocks.Add(createMockDecisionRequirementsDefinition());
		return mocks;
	  }

	  public static IList<DecisionRequirementsDefinition> createMockTwoDecisionRequirementsDefinitions()
	  {
		IList<DecisionRequirementsDefinition> mocks = new List<DecisionRequirementsDefinition>();
		mocks.Add(createMockDecisionRequirementsDefinition());
		mocks.Add(createAnotherMockDecisionRequirementsDefinition());
		return mocks;
	  }

	  // Historic job log

	  public static IList<HistoricJobLog> createMockHistoricJobLogs()
	  {
		IList<HistoricJobLog> mocks = new List<HistoricJobLog>();
		mocks.Add(createMockHistoricJobLog());
		return mocks;
	  }

	  public static HistoricJobLog createMockHistoricJobLog()
	  {
		return createMockHistoricJobLog(EXAMPLE_TENANT_ID);
	  }

	  public static HistoricJobLog createMockHistoricJobLog(string tenantId)
	  {
		HistoricJobLog mock = mock(typeof(HistoricJobLog));

		when(mock.Id).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_ID);
		when(mock.Timestamp).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_JOB_LOG_TIMESTAMP));
		when(mock.RemovalTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_JOB_LOG_REMOVAL_TIME));

		when(mock.JobId).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_JOB_ID);
		when(mock.JobDueDate).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_JOB_LOG_JOB_DUE_DATE));
		when(mock.JobRetries).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_JOB_RETRIES);
		when(mock.JobPriority).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_JOB_PRIORITY);
		when(mock.JobExceptionMessage).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_JOB_EXCEPTION_MSG);

		when(mock.JobDefinitionId).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_JOB_DEF_ID);
		when(mock.JobDefinitionType).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_JOB_DEF_TYPE);
		when(mock.JobDefinitionConfiguration).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_JOB_DEF_CONFIG);

		when(mock.ActivityId).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_ACTIVITY_ID);
		when(mock.ExecutionId).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_EXECUTION_ID);
		when(mock.ProcessInstanceId).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_PROC_INST_ID);
		when(mock.ProcessDefinitionId).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_PROC_DEF_ID);
		when(mock.ProcessDefinitionKey).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_PROC_DEF_KEY);
		when(mock.DeploymentId).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_DEPLOYMENT_ID);
		when(mock.TenantId).thenReturn(tenantId);
		when(mock.RootProcessInstanceId).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_ROOT_PROC_INST_ID);

		when(mock.CreationLog).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_IS_CREATION_LOG);
		when(mock.FailureLog).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_IS_FAILURE_LOG);
		when(mock.SuccessLog).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_IS_SUCCESS_LOG);
		when(mock.DeletionLog).thenReturn(EXAMPLE_HISTORIC_JOB_LOG_IS_DELETION_LOG);

		return mock;
	  }

	  // Historic decision instance

	  public static IList<HistoricDecisionInstance> createMockHistoricDecisionInstances()
	  {
		IList<HistoricDecisionInstance> mockList = new List<HistoricDecisionInstance>();
		mockList.Add(createMockHistoricDecisionInstance());
		return mockList;
	  }

	  public static HistoricDecisionInstance createMockHistoricDecisionInstanceBase()
	  {
		return createMockHistoricDecisionInstanceBase(EXAMPLE_TENANT_ID);
	  }

	  public static HistoricDecisionInstance createMockHistoricDecisionInstanceBase(string tenantId)
	  {
		HistoricDecisionInstance mock = mock(typeof(HistoricDecisionInstance));

		when(mock.Id).thenReturn(EXAMPLE_HISTORIC_DECISION_INSTANCE_ID);
		when(mock.DecisionDefinitionId).thenReturn(EXAMPLE_DECISION_DEFINITION_ID);
		when(mock.DecisionDefinitionKey).thenReturn(EXAMPLE_DECISION_DEFINITION_KEY);
		when(mock.DecisionDefinitionName).thenReturn(EXAMPLE_DECISION_DEFINITION_NAME);
		when(mock.ProcessDefinitionId).thenReturn(EXAMPLE_PROCESS_DEFINITION_ID);
		when(mock.ProcessDefinitionKey).thenReturn(EXAMPLE_PROCESS_DEFINITION_KEY);
		when(mock.ProcessInstanceId).thenReturn(EXAMPLE_PROCESS_INSTANCE_ID);
		when(mock.CaseDefinitionId).thenReturn(EXAMPLE_CASE_DEFINITION_ID);
		when(mock.CaseDefinitionKey).thenReturn(EXAMPLE_CASE_DEFINITION_KEY);
		when(mock.CaseInstanceId).thenReturn(EXAMPLE_CASE_INSTANCE_ID);
		when(mock.ActivityId).thenReturn(EXAMPLE_HISTORIC_DECISION_INSTANCE_ACTIVITY_ID);
		when(mock.ActivityInstanceId).thenReturn(EXAMPLE_HISTORIC_DECISION_INSTANCE_ACTIVITY_INSTANCE_ID);
		when(mock.EvaluationTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_DECISION_INSTANCE_EVALUATION_TIME));
		when(mock.RemovalTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_DECISION_INSTANCE_REMOVAL_TIME));
		when(mock.UserId).thenReturn(EXAMPLE_HISTORIC_DECISION_INSTANCE_USER_ID);
		when(mock.CollectResultValue).thenReturn(EXAMPLE_HISTORIC_DECISION_INSTANCE_COLLECT_RESULT_VALUE);
		when(mock.RootDecisionInstanceId).thenReturn(EXAMPLE_HISTORIC_DECISION_INSTANCE_ID);
		when(mock.RootProcessInstanceId).thenReturn(EXAMPLE_ROOT_HISTORIC_PROCESS_INSTANCE_ID);
		when(mock.DecisionRequirementsDefinitionId).thenReturn(EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_ID);
		when(mock.DecisionRequirementsDefinitionKey).thenReturn(EXAMPLE_DECISION_REQUIREMENTS_DEFINITION_KEY);
		when(mock.TenantId).thenReturn(tenantId);

		return mock;
	  }

	  public static HistoricDecisionInstance createMockHistoricDecisionInstance()
	  {
		HistoricDecisionInstance mock = createMockHistoricDecisionInstanceBase();
		when(mock.Inputs).thenThrow(new ProcessEngineException("ENGINE-03060 The input instances for the historic decision instance are not fetched. You must call 'includeInputs()' on the query to enable fetching."));
		when(mock.Outputs).thenThrow(new ProcessEngineException("ENGINE-03061 The output instances for the historic decision instance are not fetched. You must call 'includeOutputs()' on the query to enable fetching."));
		return mock;
	  }

	  public static HistoricDecisionInstance createMockHistoricDecisionInstanceWithInputs()
	  {
		IList<HistoricDecisionInputInstance> inputs = createMockHistoricDecisionInputInstances();

		HistoricDecisionInstance mock = createMockHistoricDecisionInstanceBase();
		when(mock.Inputs).thenReturn(inputs);
		when(mock.Outputs).thenThrow(new ProcessEngineException("ENGINE-03061 The output instances for the historic decision instance are not fetched. You must call 'includeOutputs()' on the query to enable fetching."));
		return mock;
	  }

	  public static HistoricDecisionInstance createMockHistoricDecisionInstanceWithOutputs()
	  {
		IList<HistoricDecisionOutputInstance> outputs = createMockHistoricDecisionOutputInstances();

		HistoricDecisionInstance mock = createMockHistoricDecisionInstanceBase();
		when(mock.Inputs).thenThrow(new ProcessEngineException("ENGINE-03060 The input instances for the historic decision instance are not fetched. You must call 'includeInputs()' on the query to enable fetching."));
		when(mock.Outputs).thenReturn(outputs);
		return mock;
	  }

	  public static HistoricDecisionInstance createMockHistoricDecisionInstanceWithInputsAndOutputs()
	  {
		IList<HistoricDecisionInputInstance> inputs = createMockHistoricDecisionInputInstances();
		IList<HistoricDecisionOutputInstance> outputs = createMockHistoricDecisionOutputInstances();

		HistoricDecisionInstance mock = createMockHistoricDecisionInstanceBase();
		when(mock.Inputs).thenReturn(inputs);
		when(mock.Outputs).thenReturn(outputs);
		return mock;
	  }

	  public static IList<HistoricDecisionInputInstance> createMockHistoricDecisionInputInstances()
	  {
		IList<HistoricDecisionInputInstance> mockInputs = new List<HistoricDecisionInputInstance>();
		mockInputs.Add(createMockHistoricDecisionInput(EXAMPLE_HISTORIC_DECISION_STRING_VALUE));
		mockInputs.Add(createMockHistoricDecisionInput(EXAMPLE_HISTORIC_DECISION_BYTE_ARRAY_VALUE));
		mockInputs.Add(createMockHistoricDecisionInput(EXAMPLE_HISTORIC_DECISION_SERIALIZED_VALUE));
		return mockInputs;
	  }

	  public static HistoricDecisionInputInstance createMockHistoricDecisionInput(TypedValue typedValue)
	  {
		HistoricDecisionInputInstance input = mock(typeof(HistoricDecisionInputInstance));
		when(input.Id).thenReturn(EXAMPLE_HISTORIC_DECISION_INPUT_INSTANCE_ID);
		when(input.DecisionInstanceId).thenReturn(EXAMPLE_HISTORIC_DECISION_INSTANCE_ID);
		when(input.ClauseId).thenReturn(EXAMPLE_HISTORIC_DECISION_INPUT_INSTANCE_CLAUSE_ID);
		when(input.ClauseName).thenReturn(EXAMPLE_HISTORIC_DECISION_INPUT_INSTANCE_CLAUSE_NAME);
		when(input.TypedValue).thenReturn(typedValue);
		when(input.ErrorMessage).thenReturn(null);
		when(input.CreateTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_DECISION_INPUT_INSTANCE_CREATE_TIME));
		when(input.RemovalTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_DECISION_INPUT_INSTANCE_REMOVAL_TIME));
		when(input.RootProcessInstanceId).thenReturn(EXAMPLE_HISTORIC_DECISION_INPUT_ROOT_PROCESS_INSTANCE_ID);

		return input;
	  }

	  public static IList<HistoricDecisionOutputInstance> createMockHistoricDecisionOutputInstances()
	  {
		IList<HistoricDecisionOutputInstance> mockOutputs = new List<HistoricDecisionOutputInstance>();
		mockOutputs.Add(createMockHistoricDecisionOutput(EXAMPLE_HISTORIC_DECISION_STRING_VALUE));
		mockOutputs.Add(createMockHistoricDecisionOutput(EXAMPLE_HISTORIC_DECISION_BYTE_ARRAY_VALUE));
		mockOutputs.Add(createMockHistoricDecisionOutput(EXAMPLE_HISTORIC_DECISION_SERIALIZED_VALUE));
		return mockOutputs;
	  }

	  public static HistoricDecisionOutputInstance createMockHistoricDecisionOutput(TypedValue typedValue)
	  {
		HistoricDecisionOutputInstance output = mock(typeof(HistoricDecisionOutputInstance));
		when(output.Id).thenReturn(EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_ID);
		when(output.DecisionInstanceId).thenReturn(EXAMPLE_HISTORIC_DECISION_INSTANCE_ID);
		when(output.ClauseId).thenReturn(EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_CLAUSE_ID);
		when(output.ClauseName).thenReturn(EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_CLAUSE_NAME);
		when(output.RuleId).thenReturn(EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_RULE_ID);
		when(output.RuleOrder).thenReturn(EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_RULE_ORDER);
		when(output.VariableName).thenReturn(EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_VARIABLE_NAME);
		when(output.TypedValue).thenReturn(typedValue);
		when(output.ErrorMessage).thenReturn(null);
		when(output.CreateTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_CREATE_TIME));
		when(output.RemovalTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_DECISION_OUTPUT_INSTANCE_REMOVAL_TIME));
		when(output.RootProcessInstanceId).thenReturn(EXAMPLE_HISTORIC_DECISION_OUTPUT_ROOT_PROCESS_INSTANCE_ID);
		return output;
	  }

	  // external task

	  public static MockExternalTaskBuilder mockExternalTask()
	  {
		return (new MockExternalTaskBuilder()).id(EXTERNAL_TASK_ID).activityId(EXAMPLE_ACTIVITY_ID).activityInstanceId(EXAMPLE_ACTIVITY_INSTANCE_ID).errorMessage(EXTERNAL_TASK_ERROR_MESSAGE).executionId(EXAMPLE_EXECUTION_ID).lockExpirationTime(DateTimeUtil.parseDate(EXTERNAL_TASK_LOCK_EXPIRATION_TIME)).processDefinitionId(EXAMPLE_PROCESS_DEFINITION_ID).processDefinitionKey(EXAMPLE_PROCESS_DEFINITION_KEY).processInstanceId(EXAMPLE_PROCESS_INSTANCE_ID).retries(EXTERNAL_TASK_RETRIES).suspended(EXTERNAL_TASK_SUSPENDED).topicName(EXTERNAL_TASK_TOPIC_NAME).workerId(EXTERNAL_TASK_WORKER_ID).tenantId(EXAMPLE_TENANT_ID).priority(EXTERNAL_TASK_PRIORITY).businessKey(EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY);

	  }

	  public static ExternalTask createMockExternalTask()
	  {
		return mockExternalTask().buildExternalTask();
	  }

	  public static LockedExternalTask createMockLockedExternalTask()
	  {
		return mockExternalTask().variable(EXAMPLE_VARIABLE_INSTANCE_NAME, EXAMPLE_PRIMITIVE_VARIABLE_VALUE).buildLockedExternalTask();
	  }

	  public static IList<ExternalTask> createMockExternalTasks()
	  {
		IList<ExternalTask> mocks = new List<ExternalTask>();
		mocks.Add(createMockExternalTask());
		return mocks;
	  }

	  public static MockDecisionResultBuilder mockDecisionResult()
	  {
		return (new MockDecisionResultBuilder()).resultEntries().entry(EXAMPLE_DECISION_OUTPUT_KEY, EXAMPLE_DECISION_OUTPUT_VALUE).endResultEntries();
	  }

	  public static DmnDecisionResult createMockDecisionResult()
	  {
		return mockDecisionResult().build();
	  }

	  public static MockBatchBuilder mockBatch()
	  {
		return (new MockBatchBuilder()).id(EXAMPLE_BATCH_ID).type(EXAMPLE_BATCH_TYPE).totalJobs(EXAMPLE_BATCH_TOTAL_JOBS).jobsCreated(EXAMPLE_BATCH_JOBS_CREATED).batchJobsPerSeed(EXAMPLE_BATCH_JOBS_PER_SEED).invocationsPerBatchJob(EXAMPLE_INVOCATIONS_PER_BATCH_JOB).seedJobDefinitionId(EXAMPLE_SEED_JOB_DEFINITION_ID).monitorJobDefinitionId(EXAMPLE_MONITOR_JOB_DEFINITION_ID).batchJobDefinitionId(EXAMPLE_BATCH_JOB_DEFINITION_ID).suspended().createUserId(EXAMPLE_USER_ID).tenantId(EXAMPLE_TENANT_ID);
	  }

	  public static Batch createMockBatch()
	  {
		return mockBatch().build();
	  }

	  public static IList<Batch> createMockBatches()
	  {
		IList<Batch> mockList = new List<Batch>();
		mockList.Add(createMockBatch());
		return mockList;
	  }

	  public static MockHistoricBatchBuilder mockHistoricBatch()
	  {
		return (new MockHistoricBatchBuilder()).id(EXAMPLE_BATCH_ID).type(EXAMPLE_BATCH_TYPE).totalJobs(EXAMPLE_BATCH_TOTAL_JOBS).batchJobsPerSeed(EXAMPLE_BATCH_JOBS_PER_SEED).invocationsPerBatchJob(EXAMPLE_INVOCATIONS_PER_BATCH_JOB).seedJobDefinitionId(EXAMPLE_SEED_JOB_DEFINITION_ID).monitorJobDefinitionId(EXAMPLE_MONITOR_JOB_DEFINITION_ID).batchJobDefinitionId(EXAMPLE_BATCH_JOB_DEFINITION_ID).tenantId(EXAMPLE_TENANT_ID).createUserId(EXAMPLE_USER_ID).startTime(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_BATCH_START_TIME)).endTime(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_BATCH_END_TIME)).removalTime(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_BATCH_REMOVAL_TIME));
	  }

	  public static HistoricBatch createMockHistoricBatch()
	  {
		return mockHistoricBatch().build();
	  }

	  public static IList<HistoricBatch> createMockHistoricBatches()
	  {
		IList<HistoricBatch> mockList = new List<HistoricBatch>();
		mockList.Add(createMockHistoricBatch());
		return mockList;
	  }

	  public static MockBatchStatisticsBuilder mockBatchStatistics()
	  {
		return (new MockBatchStatisticsBuilder()).id(EXAMPLE_BATCH_ID).type(EXAMPLE_BATCH_TYPE).size(EXAMPLE_BATCH_TOTAL_JOBS).jobsCreated(EXAMPLE_BATCH_JOBS_CREATED).batchJobsPerSeed(EXAMPLE_BATCH_JOBS_PER_SEED).invocationsPerBatchJob(EXAMPLE_INVOCATIONS_PER_BATCH_JOB).seedJobDefinitionId(EXAMPLE_SEED_JOB_DEFINITION_ID).monitorJobDefinitionId(EXAMPLE_MONITOR_JOB_DEFINITION_ID).batchJobDefinitionId(EXAMPLE_BATCH_JOB_DEFINITION_ID).tenantId(EXAMPLE_TENANT_ID).createUserId(EXAMPLE_USER_ID).suspended().remainingJobs(EXAMPLE_BATCH_REMAINING_JOBS).completedJobs(EXAMPLE_BATCH_COMPLETED_JOBS).failedJobs(EXAMPLE_BATCH_FAILED_JOBS);
	  }

	  public static BatchStatistics createMockBatchStatistics()
	  {
		return mockBatchStatistics().build();
	  }

	  public static IList<BatchStatistics> createMockBatchStatisticsList()
	  {
		List<BatchStatistics> mockList = new List<BatchStatistics>();
		mockList.Add(createMockBatchStatistics());
		return mockList;
	  }


	  public static MessageCorrelationResult createMessageCorrelationResult(MessageCorrelationResultType type)
	  {
		MessageCorrelationResult result = mock(typeof(MessageCorrelationResult));
		when(result.ResultType).thenReturn(type);
		if (result.ResultType.Equals(MessageCorrelationResultType.Execution))
		{
		  Execution ex = createMockExecution();
		  when(result.Execution).thenReturn(ex);
		}
		else
		{
		  ProcessInstance instance = createMockInstance();
		  when(result.ProcessInstance).thenReturn(instance);
		}
		return result;
	  }

	  public static MessageCorrelationResultWithVariables createMessageCorrelationResultWithVariables(MessageCorrelationResultType type)
	  {
		MessageCorrelationResultWithVariables result = mock(typeof(MessageCorrelationResultWithVariables));
		when(result.ResultType).thenReturn(type);
		if (result.ResultType.Equals(MessageCorrelationResultType.Execution))
		{
		  Execution ex = createMockExecution();
		  when(result.Execution).thenReturn(ex);
		}
		else
		{
		  ProcessInstance instance = createMockInstance();
		  when(result.ProcessInstance).thenReturn(instance);
		}
		when(result.Variables).thenReturn(createMockSerializedVariables());
		return result;
	  }

	  public static IList<MessageCorrelationResult> createMessageCorrelationResultList(MessageCorrelationResultType type)
	  {
		IList<MessageCorrelationResult> list = new List<MessageCorrelationResult>();
		list.Add(createMessageCorrelationResult(type));
		list.Add(createMessageCorrelationResult(type));
		return list;
	  }

	  public static IList<HistoricDecisionInstanceStatistics> createMockHistoricDecisionStatistics()
	  {
		HistoricDecisionInstanceStatistics statistics = mock(typeof(HistoricDecisionInstanceStatistics));

		when(statistics.DecisionDefinitionKey).thenReturn(EXAMPLE_DECISION_DEFINITION_KEY);
		when(statistics.Evaluations).thenReturn(1);


		HistoricDecisionInstanceStatistics anotherStatistics = mock(typeof(HistoricDecisionInstanceStatistics));

		when(anotherStatistics.DecisionDefinitionKey).thenReturn(ANOTHER_DECISION_DEFINITION_KEY);
		when(anotherStatistics.Evaluations).thenReturn(2);

		IList<HistoricDecisionInstanceStatistics> decisionResults = new List<HistoricDecisionInstanceStatistics>();
		decisionResults.Add(statistics);
		decisionResults.Add(anotherStatistics);

		return decisionResults;
	  }

	  // historic external task log
	  public static IList<HistoricExternalTaskLog> createMockHistoricExternalTaskLogs()
	  {
		IList<HistoricExternalTaskLog> mocks = new List<HistoricExternalTaskLog>();
		mocks.Add(createMockHistoricExternalTaskLog());
		return mocks;
	  }

	  public static HistoricExternalTaskLog createMockHistoricExternalTaskLog()
	  {
		return createMockHistoricExternalTaskLog(EXAMPLE_TENANT_ID);
	  }

	  public static HistoricExternalTaskLog createMockHistoricExternalTaskLog(string tenantId)
	  {
		HistoricExternalTaskLog mock = mock(typeof(HistoricExternalTaskLog));

		when(mock.Id).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ID);
		when(mock.Timestamp).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_TIMESTAMP));
		when(mock.RemovalTime).thenReturn(DateTimeUtil.parseDate(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_REMOVAL_TIME));

		when(mock.ExternalTaskId).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_EXTERNAL_TASK_ID);
		when(mock.TopicName).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_TOPIC_NAME);
		when(mock.WorkerId).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_WORKER_ID);
		when(mock.Retries).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_RETRIES);
		when(mock.Priority).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PRIORITY);
		when(mock.ErrorMessage).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ERROR_MSG);

		when(mock.ActivityId).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ACTIVITY_ID);
		when(mock.ActivityInstanceId).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ACTIVITY_INSTANCE_ID);
		when(mock.ExecutionId).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_EXECUTION_ID);
		when(mock.ProcessInstanceId).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_INST_ID);
		when(mock.ProcessDefinitionId).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_DEF_ID);
		when(mock.ProcessDefinitionKey).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_DEF_KEY);
		when(mock.TenantId).thenReturn(tenantId);
		when(mock.RootProcessInstanceId).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ROOT_PROC_INST_ID);

		when(mock.CreationLog).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_CREATION_LOG);
		when(mock.FailureLog).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_FAILURE_LOG);
		when(mock.SuccessLog).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_SUCCESS_LOG);
		when(mock.DeletionLog).thenReturn(EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_DELETION_LOG);

		return mock;
	  }
	}

}