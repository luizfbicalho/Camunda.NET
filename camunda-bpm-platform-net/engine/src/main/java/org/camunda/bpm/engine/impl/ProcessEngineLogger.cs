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
namespace org.camunda.bpm.engine.impl
{

	using ProcessApplicationLogger = org.camunda.bpm.application.impl.ProcessApplicationLogger;
	using ContainerIntegrationLogger = org.camunda.bpm.container.impl.ContainerIntegrationLogger;
	using BpmnBehaviorLogger = org.camunda.bpm.engine.impl.bpmn.behavior.BpmnBehaviorLogger;
	using BpmnParseLogger = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParseLogger;
	using ConfigurationLogger = org.camunda.bpm.engine.impl.cfg.ConfigurationLogger;
	using TransactionLogger = org.camunda.bpm.engine.impl.cfg.TransactionLogger;
	using CommandLogger = org.camunda.bpm.engine.impl.cmd.CommandLogger;
	using CmmnBehaviorLogger = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnBehaviorLogger;
	using CmmnOperationLogger = org.camunda.bpm.engine.impl.cmmn.operation.CmmnOperationLogger;
	using CmmnTransformerLogger = org.camunda.bpm.engine.impl.cmmn.transformer.CmmnTransformerLogger;
	using CoreLogger = org.camunda.bpm.engine.impl.core.CoreLogger;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using SecurityLogger = org.camunda.bpm.engine.impl.digest.SecurityLogger;
	using DecisionLogger = org.camunda.bpm.engine.impl.dmn.DecisionLogger;
	using ExternalTaskLogger = org.camunda.bpm.engine.impl.externaltask.ExternalTaskLogger;
	using IncidentLogger = org.camunda.bpm.engine.impl.incident.IncidentLogger;
	using ContextLogger = org.camunda.bpm.engine.impl.interceptor.ContextLogger;
	using JobExecutorLogger = org.camunda.bpm.engine.impl.jobexecutor.JobExecutorLogger;
	using MetricsLogger = org.camunda.bpm.engine.impl.metrics.MetricsLogger;
	using MigrationLogger = org.camunda.bpm.engine.impl.migration.MigrationLogger;
	using AdministratorAuthorizationPluginLogger = org.camunda.bpm.engine.impl.plugin.AdministratorAuthorizationPluginLogger;
	using PvmLogger = org.camunda.bpm.engine.impl.pvm.PvmLogger;
	using ScriptLogger = org.camunda.bpm.engine.impl.scripting.ScriptLogger;
	using TestLogger = org.camunda.bpm.engine.impl.test.TestLogger;
	using EngineUtilLogger = org.camunda.bpm.engine.impl.util.EngineUtilLogger;
	using BaseLogger = org.camunda.commons.logging.BaseLogger;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class ProcessEngineLogger : BaseLogger
	{

	  public const string PROJECT_CODE = "ENGINE";

	  public static readonly ProcessEngineLogger INSTANCE = BaseLogger.createLogger(typeof(ProcessEngineLogger), PROJECT_CODE, "org.camunda.bpm.engine", "00");

	  public static readonly BpmnParseLogger BPMN_PARSE_LOGGER = BaseLogger.createLogger(typeof(BpmnParseLogger), PROJECT_CODE, "org.camunda.bpm.engine.bpmn.parser", "01");

	  public static readonly BpmnBehaviorLogger BPMN_BEHAVIOR_LOGGER = BaseLogger.createLogger(typeof(BpmnBehaviorLogger), PROJECT_CODE, "org.camunda.bpm.engine.bpmn.behavior", "02");

	  public static readonly EnginePersistenceLogger PERSISTENCE_LOGGER = BaseLogger.createLogger(typeof(EnginePersistenceLogger), PROJECT_CODE, "org.camunda.bpm.engine.persistence", "03");

	  public static readonly CmmnTransformerLogger CMMN_TRANSFORMER_LOGGER = BaseLogger.createLogger(typeof(CmmnTransformerLogger), PROJECT_CODE, "org.camunda.bpm.engine.cmmn.transformer", "04");

	  public static readonly CmmnBehaviorLogger CMNN_BEHAVIOR_LOGGER = BaseLogger.createLogger(typeof(CmmnBehaviorLogger), PROJECT_CODE, "org.camunda.bpm.engine.cmmn.behavior", "05");

	  public static readonly CmmnOperationLogger CMMN_OPERATION_LOGGER = BaseLogger.createLogger(typeof(CmmnOperationLogger), PROJECT_CODE, "org.camunda.bpm.engine.cmmn.operation", "06");

	  public static readonly ProcessApplicationLogger PROCESS_APPLICATION_LOGGER = BaseLogger.createLogger(typeof(ProcessApplicationLogger), PROJECT_CODE, "org.camunda.bpm.application", "07");

	  public static readonly ContainerIntegrationLogger CONTAINER_INTEGRATION_LOGGER = BaseLogger.createLogger(typeof(ContainerIntegrationLogger), PROJECT_CODE, "org.camunda.bpm.container", "08");

	  public static readonly EngineUtilLogger UTIL_LOGGER = BaseLogger.createLogger(typeof(EngineUtilLogger), PROJECT_CODE, "org.camunda.bpm.engine.util", "09");

	  public static readonly TransactionLogger TX_LOGGER = BaseLogger.createLogger(typeof(TransactionLogger), PROJECT_CODE, "org.camunda.bpm.engine.tx", "11");

	  public static readonly ConfigurationLogger CONFIG_LOGGER = BaseLogger.createLogger(typeof(ConfigurationLogger), PROJECT_CODE, "org.camunda.bpm.engine.cfg", "12");

	  public static readonly CommandLogger CMD_LOGGER = BaseLogger.createLogger(typeof(CommandLogger), PROJECT_CODE, "org.camunda.bpm.engine.cmd", "13");

	  public static readonly JobExecutorLogger JOB_EXECUTOR_LOGGER = BaseLogger.createLogger(typeof(JobExecutorLogger), PROJECT_CODE, "org.camunda.bpm.engine.jobexecutor", "14");

	  public static readonly TestLogger TEST_LOGGER = BaseLogger.createLogger(typeof(TestLogger), PROJECT_CODE, "org.camunda.bpm.engine.test", "15");

	  public static readonly ContextLogger CONTEXT_LOGGER = BaseLogger.createLogger(typeof(ContextLogger), PROJECT_CODE, "org.camunda.bpm.engine.context", "16");

	  public static readonly CoreLogger CORE_LOGGER = BaseLogger.createLogger(typeof(CoreLogger), PROJECT_CODE, "org.camunda.bpm.engine.core", "17");

	  public static readonly MetricsLogger METRICS_LOGGER = BaseLogger.createLogger(typeof(MetricsLogger), PROJECT_CODE, "org.camunda.bpm.engine.metrics", "18");

	  public static readonly AdministratorAuthorizationPluginLogger ADMIN_PLUGIN_LOGGER = BaseLogger.createLogger(typeof(AdministratorAuthorizationPluginLogger), PROJECT_CODE, "org.camunda.bpm.engine.plugin.admin", "19");

	  public static readonly PvmLogger PVM_LOGGER = BaseLogger.createLogger(typeof(PvmLogger), PROJECT_CODE, "org.camunda.bpm.engine.pvm", "20");

	  public static readonly ScriptLogger SCRIPT_LOGGER = BaseLogger.createLogger(typeof(ScriptLogger), PROJECT_CODE, "org.camunda.bpm.engine.script", "21");

	  public static readonly DecisionLogger DECISION_LOGGER = BaseLogger.createLogger(typeof(DecisionLogger), PROJECT_CODE, "org.camunda.bpm.engine.dmn", "22");

	  public static readonly MigrationLogger MIGRATION_LOGGER = BaseLogger.createLogger(typeof(MigrationLogger), PROJECT_CODE, "org.camunda.bpm.engine.migration", "23");

	  public static readonly ExternalTaskLogger EXTERNAL_TASK_LOGGER = BaseLogger.createLogger(typeof(ExternalTaskLogger), PROJECT_CODE, "org.camunda.bpm.engine.externaltask", "24");

	  public static readonly SecurityLogger SECURITY_LOGGER = BaseLogger.createLogger(typeof(SecurityLogger), PROJECT_CODE, "org.camunda.bpm.engine.security", "25");

	  public static readonly IncidentLogger INCIDENT_LOGGER = BaseLogger.createLogger(typeof(IncidentLogger), PROJECT_CODE, "org.camunda.bpm.engine.incident", "26");

	  public virtual void processEngineCreated(string name)
	  {
		logInfo("001", "Process Engine {} created.", name);
	  }

	  public virtual void processEngineAlreadyInitialized()
	  {
		logInfo("002", "Process engine already initialized");
	  }

	  public virtual void initializingProcessEngineForResource(URL resourceUrl)
	  {
		logInfo("003", "Initializing process engine for resource {}", resourceUrl);
	  }

	  public virtual void initializingProcessEngine(string name)
	  {
		logInfo("004", "Initializing process engine {}", name);
	  }

	  public virtual void exceptionWhileInitializingProcessengine(Exception e)
	  {
		logError("005", "Exception while initializing process engine {}", e.Message, e);
	  }

	  public virtual void exceptionWhileClosingProcessEngine(string @string, Exception e)
	  {
		logError("006", "Exception while closing process engine {}", @string, e);
	  }

	  public virtual void processEngineClosed(string name)
	  {
		logInfo("007", "Process Engine {} closed", name);
	  }

	  public virtual void historyCleanupJobReconfigurationFailure(Exception exception)
	  {
		logInfo("008", "History Cleanup Job reconfiguration failed on Process Engine Bootstrap. Possible concurrent execution with the JobExecutor: {}", exception.Message);
	  }

	}


}