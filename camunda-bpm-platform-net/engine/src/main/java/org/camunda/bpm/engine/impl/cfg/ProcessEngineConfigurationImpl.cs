using System;
using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.impl.cfg
{

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmd.HistoryCleanupCmd.MAX_THREADS_NUMBER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;



	using XMLConfigBuilder = org.apache.ibatis.builder.xml.XMLConfigBuilder;
	using PooledDataSource = org.apache.ibatis.datasource.pooled.PooledDataSource;
	using Environment = org.apache.ibatis.mapping.Environment;
	using Configuration = org.apache.ibatis.session.Configuration;
	using ExecutorType = org.apache.ibatis.session.ExecutorType;
	using SqlSessionFactory = org.apache.ibatis.session.SqlSessionFactory;
	using DefaultSqlSessionFactory = org.apache.ibatis.session.defaults.DefaultSqlSessionFactory;
	using TransactionFactory = org.apache.ibatis.transaction.TransactionFactory;
	using JdbcTransactionFactory = org.apache.ibatis.transaction.jdbc.JdbcTransactionFactory;
	using ManagedTransactionFactory = org.apache.ibatis.transaction.managed.ManagedTransactionFactory;
	using DmnEngine = org.camunda.bpm.dmn.engine.DmnEngine;
	using DmnEngineConfiguration = org.camunda.bpm.dmn.engine.DmnEngineConfiguration;
	using DefaultDmnEngineConfiguration = org.camunda.bpm.dmn.engine.impl.DefaultDmnEngineConfiguration;
	using Groups = org.camunda.bpm.engine.authorization.Groups;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using ProcessApplicationManager = org.camunda.bpm.engine.impl.application.ProcessApplicationManager;
	using BatchSetRemovalTimeJobHandler = org.camunda.bpm.engine.impl.batch.removaltime.BatchSetRemovalTimeJobHandler;
	using DecisionSetRemovalTimeJobHandler = org.camunda.bpm.engine.impl.batch.removaltime.DecisionSetRemovalTimeJobHandler;
	using ProcessSetRemovalTimeJobHandler = org.camunda.bpm.engine.impl.batch.removaltime.ProcessSetRemovalTimeJobHandler;
	using BatchJobHandler = org.camunda.bpm.engine.impl.batch.BatchJobHandler;
	using BatchMonitorJobHandler = org.camunda.bpm.engine.impl.batch.BatchMonitorJobHandler;
	using BatchSeedJobHandler = org.camunda.bpm.engine.impl.batch.BatchSeedJobHandler;
	using DeleteHistoricProcessInstancesJobHandler = org.camunda.bpm.engine.impl.batch.deletion.DeleteHistoricProcessInstancesJobHandler;
	using DeleteProcessInstancesJobHandler = org.camunda.bpm.engine.impl.batch.deletion.DeleteProcessInstancesJobHandler;
	using SetExternalTaskRetriesJobHandler = org.camunda.bpm.engine.impl.batch.externaltask.SetExternalTaskRetriesJobHandler;
	using SetJobRetriesJobHandler = org.camunda.bpm.engine.impl.batch.job.SetJobRetriesJobHandler;
	using UpdateProcessInstancesSuspendStateJobHandler = org.camunda.bpm.engine.impl.batch.update.UpdateProcessInstancesSuspendStateJobHandler;
	using ExternalTaskActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.ExternalTaskActivityBehavior;
	using BpmnDeployer = org.camunda.bpm.engine.impl.bpmn.deployer.BpmnDeployer;
	using BpmnParseListener = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParseListener;
	using BpmnParser = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParser;
	using DefaultFailedJobParseListener = org.camunda.bpm.engine.impl.bpmn.parser.DefaultFailedJobParseListener;
	using BusinessCalendarManager = org.camunda.bpm.engine.impl.calendar.BusinessCalendarManager;
	using CycleBusinessCalendar = org.camunda.bpm.engine.impl.calendar.CycleBusinessCalendar;
	using DueDateBusinessCalendar = org.camunda.bpm.engine.impl.calendar.DueDateBusinessCalendar;
	using DurationBusinessCalendar = org.camunda.bpm.engine.impl.calendar.DurationBusinessCalendar;
	using MapBusinessCalendarManager = org.camunda.bpm.engine.impl.calendar.MapBusinessCalendarManager;
	using AuthorizationCommandChecker = org.camunda.bpm.engine.impl.cfg.auth.AuthorizationCommandChecker;
	using DefaultAuthorizationProvider = org.camunda.bpm.engine.impl.cfg.auth.DefaultAuthorizationProvider;
	using DefaultPermissionProvider = org.camunda.bpm.engine.impl.cfg.auth.DefaultPermissionProvider;
	using PermissionProvider = org.camunda.bpm.engine.impl.cfg.auth.PermissionProvider;
	using ResourceAuthorizationProvider = org.camunda.bpm.engine.impl.cfg.auth.ResourceAuthorizationProvider;
	using TenantCommandChecker = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantCommandChecker;
	using TenantIdProvider = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProvider;
	using StandaloneTransactionContextFactory = org.camunda.bpm.engine.impl.cfg.standalone.StandaloneTransactionContextFactory;
	using HistoryCleanupCmd = org.camunda.bpm.engine.impl.cmd.HistoryCleanupCmd;
	using CaseServiceImpl = org.camunda.bpm.engine.impl.cmmn.CaseServiceImpl;
	using CmmnDeployer = org.camunda.bpm.engine.impl.cmmn.deployer.CmmnDeployer;
	using CaseDefinitionManager = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionManager;
	using CaseExecutionManager = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionManager;
	using CaseSentryPartManager = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseSentryPartManager;
	using DefaultCmmnElementHandlerRegistry = org.camunda.bpm.engine.impl.cmmn.handler.DefaultCmmnElementHandlerRegistry;
	using CmmnTransformFactory = org.camunda.bpm.engine.impl.cmmn.transformer.CmmnTransformFactory;
	using CmmnTransformListener = org.camunda.bpm.engine.impl.cmmn.transformer.CmmnTransformListener;
	using CmmnTransformer = org.camunda.bpm.engine.impl.cmmn.transformer.CmmnTransformer;
	using DefaultCmmnTransformFactory = org.camunda.bpm.engine.impl.cmmn.transformer.DefaultCmmnTransformFactory;
	using DbIdGenerator = org.camunda.bpm.engine.impl.db.DbIdGenerator;
	using DbEntityManagerFactory = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManagerFactory;
	using DbEntityCacheKeyMapping = org.camunda.bpm.engine.impl.db.entitymanager.cache.DbEntityCacheKeyMapping;
	using DbSqlPersistenceProviderFactory = org.camunda.bpm.engine.impl.db.sql.DbSqlPersistenceProviderFactory;
	using DbSqlSessionFactory = org.camunda.bpm.engine.impl.db.sql.DbSqlSessionFactory;
	using DefaultDelegateInterceptor = org.camunda.bpm.engine.impl.@delegate.DefaultDelegateInterceptor;
	using Default16ByteSaltGenerator = org.camunda.bpm.engine.impl.digest.Default16ByteSaltGenerator;
	using PasswordEncryptor = org.camunda.bpm.engine.impl.digest.PasswordEncryptor;
	using PasswordManager = org.camunda.bpm.engine.impl.digest.PasswordManager;
	using SaltGenerator = org.camunda.bpm.engine.impl.digest.SaltGenerator;
	using Sha512HashDigest = org.camunda.bpm.engine.impl.digest.Sha512HashDigest;
	using DeleteHistoricDecisionInstancesJobHandler = org.camunda.bpm.engine.impl.dmn.batch.DeleteHistoricDecisionInstancesJobHandler;
	using DmnEngineConfigurationBuilder = org.camunda.bpm.engine.impl.dmn.configuration.DmnEngineConfigurationBuilder;
	using DecisionDefinitionDeployer = org.camunda.bpm.engine.impl.dmn.deployer.DecisionDefinitionDeployer;
	using DecisionRequirementsDefinitionDeployer = org.camunda.bpm.engine.impl.dmn.deployer.DecisionRequirementsDefinitionDeployer;
	using DecisionDefinitionManager = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionManager;
	using DecisionRequirementsDefinitionManager = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionManager;
	using CommandContextFunctionMapper = org.camunda.bpm.engine.impl.el.CommandContextFunctionMapper;
	using DateTimeFunctionMapper = org.camunda.bpm.engine.impl.el.DateTimeFunctionMapper;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using CompensationEventHandler = org.camunda.bpm.engine.impl.@event.CompensationEventHandler;
	using ConditionalEventHandler = org.camunda.bpm.engine.impl.@event.ConditionalEventHandler;
	using EventHandler = org.camunda.bpm.engine.impl.@event.EventHandler;
	using EventHandlerImpl = org.camunda.bpm.engine.impl.@event.EventHandlerImpl;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using SignalEventHandler = org.camunda.bpm.engine.impl.@event.SignalEventHandler;
	using DefaultExternalTaskPriorityProvider = org.camunda.bpm.engine.impl.externaltask.DefaultExternalTaskPriorityProvider;
	using FormEngine = org.camunda.bpm.engine.impl.form.engine.FormEngine;
	using HtmlFormEngine = org.camunda.bpm.engine.impl.form.engine.HtmlFormEngine;
	using JuelFormEngine = org.camunda.bpm.engine.impl.form.engine.JuelFormEngine;
	using AbstractFormFieldType = org.camunda.bpm.engine.impl.form.type.AbstractFormFieldType;
	using BooleanFormType = org.camunda.bpm.engine.impl.form.type.BooleanFormType;
	using DateFormType = org.camunda.bpm.engine.impl.form.type.DateFormType;
	using FormTypes = org.camunda.bpm.engine.impl.form.type.FormTypes;
	using LongFormType = org.camunda.bpm.engine.impl.form.type.LongFormType;
	using StringFormType = org.camunda.bpm.engine.impl.form.type.StringFormType;
	using FormFieldValidator = org.camunda.bpm.engine.impl.form.validator.FormFieldValidator;
	using FormValidators = org.camunda.bpm.engine.impl.form.validator.FormValidators;
	using MaxLengthValidator = org.camunda.bpm.engine.impl.form.validator.MaxLengthValidator;
	using MaxValidator = org.camunda.bpm.engine.impl.form.validator.MaxValidator;
	using MinLengthValidator = org.camunda.bpm.engine.impl.form.validator.MinLengthValidator;
	using MinValidator = org.camunda.bpm.engine.impl.form.validator.MinValidator;
	using ReadOnlyValidator = org.camunda.bpm.engine.impl.form.validator.ReadOnlyValidator;
	using RequiredValidator = org.camunda.bpm.engine.impl.form.validator.RequiredValidator;
	using DefaultHistoryRemovalTimeProvider = org.camunda.bpm.engine.impl.history.DefaultHistoryRemovalTimeProvider;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryRemovalTimeProvider = org.camunda.bpm.engine.impl.history.HistoryRemovalTimeProvider;
	using HistoricDecisionInstanceManager = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionInstanceManager;
	using DbHistoryEventHandler = org.camunda.bpm.engine.impl.history.handler.DbHistoryEventHandler;
	using HistoryEventHandler = org.camunda.bpm.engine.impl.history.handler.HistoryEventHandler;
	using HistoryParseListener = org.camunda.bpm.engine.impl.history.parser.HistoryParseListener;
	using CacheAwareCmmnHistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.CacheAwareCmmnHistoryEventProducer;
	using CacheAwareHistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.CacheAwareHistoryEventProducer;
	using CmmnHistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.CmmnHistoryEventProducer;
	using DefaultDmnHistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.DefaultDmnHistoryEventProducer;
	using DmnHistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.DmnHistoryEventProducer;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using CmmnHistoryTransformListener = org.camunda.bpm.engine.impl.history.transformer.CmmnHistoryTransformListener;
	using DefaultPasswordPolicyImpl = org.camunda.bpm.engine.impl.identity.DefaultPasswordPolicyImpl;
	using ReadOnlyIdentityProvider = org.camunda.bpm.engine.impl.identity.ReadOnlyIdentityProvider;
	using WritableIdentityProvider = org.camunda.bpm.engine.impl.identity.WritableIdentityProvider;
	using DbIdentityServiceProvider = org.camunda.bpm.engine.impl.identity.db.DbIdentityServiceProvider;
	using DefaultIncidentHandler = org.camunda.bpm.engine.impl.incident.DefaultIncidentHandler;
	using IncidentHandler = org.camunda.bpm.engine.impl.incident.IncidentHandler;
	using CommandContextFactory = org.camunda.bpm.engine.impl.interceptor.CommandContextFactory;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using CommandExecutorImpl = org.camunda.bpm.engine.impl.interceptor.CommandExecutorImpl;
	using CommandInterceptor = org.camunda.bpm.engine.impl.interceptor.CommandInterceptor;
	using DelegateInterceptor = org.camunda.bpm.engine.impl.interceptor.DelegateInterceptor;
	using SessionFactory = org.camunda.bpm.engine.impl.interceptor.SessionFactory;
	using AsyncContinuationJobHandler = org.camunda.bpm.engine.impl.jobexecutor.AsyncContinuationJobHandler;
	using DefaultFailedJobCommandFactory = org.camunda.bpm.engine.impl.jobexecutor.DefaultFailedJobCommandFactory;
	using DefaultJobExecutor = org.camunda.bpm.engine.impl.jobexecutor.DefaultJobExecutor;
	using DefaultJobPriorityProvider = org.camunda.bpm.engine.impl.jobexecutor.DefaultJobPriorityProvider;
	using FailedJobCommandFactory = org.camunda.bpm.engine.impl.jobexecutor.FailedJobCommandFactory;
	using JobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using JobHandler = org.camunda.bpm.engine.impl.jobexecutor.JobHandler;
	using NotifyAcquisitionRejectedJobsHandler = org.camunda.bpm.engine.impl.jobexecutor.NotifyAcquisitionRejectedJobsHandler;
	using ProcessEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.ProcessEventJobHandler;
	using RejectedJobsHandler = org.camunda.bpm.engine.impl.jobexecutor.RejectedJobsHandler;
	using TimerActivateJobDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerActivateJobDefinitionHandler;
	using TimerActivateProcessDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerActivateProcessDefinitionHandler;
	using TimerCatchIntermediateEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerCatchIntermediateEventJobHandler;
	using TimerExecuteNestedActivityJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerExecuteNestedActivityJobHandler;
	using TimerStartEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerStartEventJobHandler;
	using TimerStartEventSubprocessJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerStartEventSubprocessJobHandler;
	using TimerSuspendJobDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerSuspendJobDefinitionHandler;
	using TimerSuspendProcessDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerSuspendProcessDefinitionHandler;
	using BatchWindowManager = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.BatchWindowManager;
	using DefaultBatchWindowManager = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.DefaultBatchWindowManager;
	using HistoryCleanupBatch = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupBatch;
	using HistoryCleanupHandler = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupHandler;
	using HistoryCleanupHelper = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupHelper;
	using HistoryCleanupJobHandler = org.camunda.bpm.engine.impl.jobexecutor.historycleanup.HistoryCleanupJobHandler;
	using MetricsRegistry = org.camunda.bpm.engine.impl.metrics.MetricsRegistry;
	using MetricsReporterIdProvider = org.camunda.bpm.engine.impl.metrics.MetricsReporterIdProvider;
	using SimpleIpBasedProvider = org.camunda.bpm.engine.impl.metrics.SimpleIpBasedProvider;
	using MetricsBpmnParseListener = org.camunda.bpm.engine.impl.metrics.parser.MetricsBpmnParseListener;
	using MetricsCmmnTransformListener = org.camunda.bpm.engine.impl.metrics.parser.MetricsCmmnTransformListener;
	using DbMetricsReporter = org.camunda.bpm.engine.impl.metrics.reporter.DbMetricsReporter;
	using DefaultMigrationActivityMatcher = org.camunda.bpm.engine.impl.migration.DefaultMigrationActivityMatcher;
	using DefaultMigrationInstructionGenerator = org.camunda.bpm.engine.impl.migration.DefaultMigrationInstructionGenerator;
	using MigrationActivityMatcher = org.camunda.bpm.engine.impl.migration.MigrationActivityMatcher;
	using MigrationInstructionGenerator = org.camunda.bpm.engine.impl.migration.MigrationInstructionGenerator;
	using MigrationBatchJobHandler = org.camunda.bpm.engine.impl.migration.batch.MigrationBatchJobHandler;
	using MigrationActivityValidator = org.camunda.bpm.engine.impl.migration.validation.activity.MigrationActivityValidator;
	using NoCompensationHandlerActivityValidator = org.camunda.bpm.engine.impl.migration.validation.activity.NoCompensationHandlerActivityValidator;
	using SupportedActivityValidator = org.camunda.bpm.engine.impl.migration.validation.activity.SupportedActivityValidator;
	using SupportedPassiveEventTriggerActivityValidator = org.camunda.bpm.engine.impl.migration.validation.activity.SupportedPassiveEventTriggerActivityValidator;
	using AsyncAfterMigrationValidator = org.camunda.bpm.engine.impl.migration.validation.instance.AsyncAfterMigrationValidator;
	using AsyncMigrationValidator = org.camunda.bpm.engine.impl.migration.validation.instance.AsyncMigrationValidator;
	using AsyncProcessStartMigrationValidator = org.camunda.bpm.engine.impl.migration.validation.instance.AsyncProcessStartMigrationValidator;
	using MigratingActivityInstanceValidator = org.camunda.bpm.engine.impl.migration.validation.instance.MigratingActivityInstanceValidator;
	using MigratingCompensationInstanceValidator = org.camunda.bpm.engine.impl.migration.validation.instance.MigratingCompensationInstanceValidator;
	using MigratingTransitionInstanceValidator = org.camunda.bpm.engine.impl.migration.validation.instance.MigratingTransitionInstanceValidator;
	using NoUnmappedCompensationStartEventValidator = org.camunda.bpm.engine.impl.migration.validation.instance.NoUnmappedCompensationStartEventValidator;
	using NoUnmappedLeafInstanceValidator = org.camunda.bpm.engine.impl.migration.validation.instance.NoUnmappedLeafInstanceValidator;
	using SupportedActivityInstanceValidator = org.camunda.bpm.engine.impl.migration.validation.instance.SupportedActivityInstanceValidator;
	using VariableConflictActivityInstanceValidator = org.camunda.bpm.engine.impl.migration.validation.instance.VariableConflictActivityInstanceValidator;
	using AdditionalFlowScopeInstructionValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.AdditionalFlowScopeInstructionValidator;
	using CannotAddMultiInstanceBodyValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.CannotAddMultiInstanceBodyValidator;
	using CannotAddMultiInstanceInnerActivityValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.CannotAddMultiInstanceInnerActivityValidator;
	using CannotRemoveMultiInstanceInnerActivityValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.CannotRemoveMultiInstanceInnerActivityValidator;
	using ConditionalEventUpdateEventTriggerValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.ConditionalEventUpdateEventTriggerValidator;
	using GatewayMappingValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.GatewayMappingValidator;
	using MigrationInstructionValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.MigrationInstructionValidator;
	using OnlyOnceMappedActivityInstructionValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.OnlyOnceMappedActivityInstructionValidator;
	using SameBehaviorInstructionValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.SameBehaviorInstructionValidator;
	using SameEventScopeInstructionValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.SameEventScopeInstructionValidator;
	using SameEventTypeValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.SameEventTypeValidator;
	using UpdateEventTriggersValidator = org.camunda.bpm.engine.impl.migration.validation.instruction.UpdateEventTriggersValidator;
	using OptimizeManager = org.camunda.bpm.engine.impl.optimize.OptimizeManager;
	using GenericManagerFactory = org.camunda.bpm.engine.impl.persistence.GenericManagerFactory;
	using Deployer = org.camunda.bpm.engine.impl.persistence.deploy.Deployer;
	using CacheFactory = org.camunda.bpm.engine.impl.persistence.deploy.cache.CacheFactory;
	using DefaultCacheFactory = org.camunda.bpm.engine.impl.persistence.deploy.cache.DefaultCacheFactory;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using AttachmentManager = org.camunda.bpm.engine.impl.persistence.entity.AttachmentManager;
	using AuthorizationManager = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationManager;
	using BatchManager = org.camunda.bpm.engine.impl.persistence.entity.BatchManager;
	using ByteArrayManager = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayManager;
	using CommentManager = org.camunda.bpm.engine.impl.persistence.entity.CommentManager;
	using DeploymentManager = org.camunda.bpm.engine.impl.persistence.entity.DeploymentManager;
	using EventSubscriptionManager = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionManager;
	using ExecutionManager = org.camunda.bpm.engine.impl.persistence.entity.ExecutionManager;
	using ExternalTaskManager = org.camunda.bpm.engine.impl.persistence.entity.ExternalTaskManager;
	using FilterManager = org.camunda.bpm.engine.impl.persistence.entity.FilterManager;
	using HistoricActivityInstanceManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricActivityInstanceManager;
	using HistoricBatchManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricBatchManager;
	using HistoricCaseActivityInstanceManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricCaseActivityInstanceManager;
	using HistoricCaseInstanceManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricCaseInstanceManager;
	using HistoricDetailManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricDetailManager;
	using HistoricExternalTaskLogManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricExternalTaskLogManager;
	using HistoricIdentityLinkLogManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricIdentityLinkLogManager;
	using HistoricIncidentManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricIncidentManager;
	using HistoricJobLogManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricJobLogManager;
	using HistoricProcessInstanceManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricProcessInstanceManager;
	using HistoricStatisticsManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricStatisticsManager;
	using HistoricTaskInstanceManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricTaskInstanceManager;
	using HistoricVariableInstanceManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricVariableInstanceManager;
	using IdentityInfoManager = org.camunda.bpm.engine.impl.persistence.entity.IdentityInfoManager;
	using IdentityLinkManager = org.camunda.bpm.engine.impl.persistence.entity.IdentityLinkManager;
	using IncidentManager = org.camunda.bpm.engine.impl.persistence.entity.IncidentManager;
	using JobDefinitionManager = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionManager;
	using JobManager = org.camunda.bpm.engine.impl.persistence.entity.JobManager;
	using MeterLogManager = org.camunda.bpm.engine.impl.persistence.entity.MeterLogManager;
	using ProcessDefinitionManager = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionManager;
	using PropertyManager = org.camunda.bpm.engine.impl.persistence.entity.PropertyManager;
	using ReportManager = org.camunda.bpm.engine.impl.persistence.entity.ReportManager;
	using ResourceManager = org.camunda.bpm.engine.impl.persistence.entity.ResourceManager;
	using SchemaLogManager = org.camunda.bpm.engine.impl.persistence.entity.SchemaLogManager;
	using StatisticsManager = org.camunda.bpm.engine.impl.persistence.entity.StatisticsManager;
	using TableDataManager = org.camunda.bpm.engine.impl.persistence.entity.TableDataManager;
	using TaskManager = org.camunda.bpm.engine.impl.persistence.entity.TaskManager;
	using TaskReportManager = org.camunda.bpm.engine.impl.persistence.entity.TaskReportManager;
	using TenantManager = org.camunda.bpm.engine.impl.persistence.entity.TenantManager;
	using UserOperationLogManager = org.camunda.bpm.engine.impl.persistence.entity.UserOperationLogManager;
	using VariableInstanceManager = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceManager;
	using ConditionHandler = org.camunda.bpm.engine.impl.runtime.ConditionHandler;
	using CorrelationHandler = org.camunda.bpm.engine.impl.runtime.CorrelationHandler;
	using DefaultConditionHandler = org.camunda.bpm.engine.impl.runtime.DefaultConditionHandler;
	using DefaultCorrelationHandler = org.camunda.bpm.engine.impl.runtime.DefaultCorrelationHandler;
	using ScriptFactory = org.camunda.bpm.engine.impl.scripting.ScriptFactory;
	using BeansResolverFactory = org.camunda.bpm.engine.impl.scripting.engine.BeansResolverFactory;
	using ResolverFactory = org.camunda.bpm.engine.impl.scripting.engine.ResolverFactory;
	using ScriptBindingsFactory = org.camunda.bpm.engine.impl.scripting.engine.ScriptBindingsFactory;
	using ScriptingEngines = org.camunda.bpm.engine.impl.scripting.engine.ScriptingEngines;
	using VariableScopeResolverFactory = org.camunda.bpm.engine.impl.scripting.engine.VariableScopeResolverFactory;
	using ScriptEnvResolver = org.camunda.bpm.engine.impl.scripting.env.ScriptEnvResolver;
	using ScriptingEnvironment = org.camunda.bpm.engine.impl.scripting.env.ScriptingEnvironment;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using ParseUtil = org.camunda.bpm.engine.impl.util.ParseUtil;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using ValueTypeResolverImpl = org.camunda.bpm.engine.impl.variable.ValueTypeResolverImpl;
	using BooleanValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.BooleanValueSerializer;
	using ByteArrayValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.ByteArrayValueSerializer;
	using DateValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.DateValueSerializer;
	using DefaultVariableSerializers = org.camunda.bpm.engine.impl.variable.serializer.DefaultVariableSerializers;
	using DoubleValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.DoubleValueSerializer;
	using FileValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.FileValueSerializer;
	using IntegerValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.IntegerValueSerializer;
	using JavaObjectSerializer = org.camunda.bpm.engine.impl.variable.serializer.JavaObjectSerializer;
	using LongValueSerlializer = org.camunda.bpm.engine.impl.variable.serializer.LongValueSerlializer;
	using NullValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.NullValueSerializer;
	using ShortValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.ShortValueSerializer;
	using StringValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.StringValueSerializer;
	using TypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer;
	using VariableSerializerFactory = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializerFactory;
	using VariableSerializers = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializers;
	using EntityManagerSession = org.camunda.bpm.engine.impl.variable.serializer.jpa.EntityManagerSession;
	using EntityManagerSessionFactory = org.camunda.bpm.engine.impl.variable.serializer.jpa.EntityManagerSessionFactory;
	using JPAVariableSerializer = org.camunda.bpm.engine.impl.variable.serializer.jpa.JPAVariableSerializer;
	using Metrics = org.camunda.bpm.engine.management.Metrics;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using MocksResolverFactory = org.camunda.bpm.engine.test.mock.MocksResolverFactory;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public abstract class ProcessEngineConfigurationImpl : ProcessEngineConfiguration
	{

	  protected internal static readonly ConfigurationLogger LOG = ConfigurationLogger.CONFIG_LOGGER;

	  public const string DB_SCHEMA_UPDATE_CREATE = "create";
	  public const string DB_SCHEMA_UPDATE_DROP_CREATE = "drop-create";

	  public static readonly int HISTORYLEVEL_NONE = org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_NONE.Id;
	  public static readonly int HISTORYLEVEL_ACTIVITY = org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_ACTIVITY.Id;
	  public static readonly int HISTORYLEVEL_AUDIT = org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_AUDIT.Id;
	  public static readonly int HISTORYLEVEL_FULL = org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL.Id;

	  public const string DEFAULT_WS_SYNC_FACTORY = "org.camunda.bpm.engine.impl.webservice.CxfWebServiceClientFactory";

	  public const string DEFAULT_MYBATIS_MAPPING_FILE = "org/camunda/bpm/engine/impl/mapping/mappings.xml";

	  public const int DEFAULT_FAILED_JOB_LISTENER_MAX_RETRIES = 3;

	  public static SqlSessionFactory cachedSqlSessionFactory;

	  // SERVICES /////////////////////////////////////////////////////////////////

	  protected internal RepositoryService repositoryService = new RepositoryServiceImpl();
	  protected internal RuntimeService runtimeService = new RuntimeServiceImpl();
	  protected internal HistoryService historyService = new HistoryServiceImpl();
	  protected internal IdentityService identityService = new IdentityServiceImpl();
	  protected internal TaskService taskService = new TaskServiceImpl();
	  protected internal FormService formService = new FormServiceImpl();
	  protected internal ManagementService managementService = new ManagementServiceImpl();
	  protected internal AuthorizationService authorizationService = new AuthorizationServiceImpl();
	  protected internal CaseService caseService = new CaseServiceImpl();
	  protected internal FilterService filterService = new FilterServiceImpl();
	  protected internal ExternalTaskService externalTaskService = new ExternalTaskServiceImpl();
	  protected internal DecisionService decisionService = new DecisionServiceImpl();
	  protected internal OptimizeService optimizeService = new OptimizeService();

	  // COMMAND EXECUTORS ////////////////////////////////////////////////////////

	  // Command executor and interceptor stack
	  /// <summary>
	  /// the configurable list which will be <seealso cref="initInterceptorChain(System.Collections.IList) processed"/> to build the <seealso cref="commandExecutorTxRequired"/>
	  /// </summary>
	  protected internal IList<CommandInterceptor> customPreCommandInterceptorsTxRequired;
	  protected internal IList<CommandInterceptor> customPostCommandInterceptorsTxRequired;

	  protected internal IList<CommandInterceptor> commandInterceptorsTxRequired;

	  /// <summary>
	  /// this will be initialized during the configurationComplete()
	  /// </summary>
	  protected internal CommandExecutor commandExecutorTxRequired;

	  /// <summary>
	  /// the configurable list which will be <seealso cref="initInterceptorChain(System.Collections.IList) processed"/> to build the <seealso cref="commandExecutorTxRequiresNew"/>
	  /// </summary>
	  protected internal IList<CommandInterceptor> customPreCommandInterceptorsTxRequiresNew;
	  protected internal IList<CommandInterceptor> customPostCommandInterceptorsTxRequiresNew;

	  protected internal IList<CommandInterceptor> commandInterceptorsTxRequiresNew;

	  /// <summary>
	  /// this will be initialized during the configurationComplete()
	  /// </summary>
	  protected internal CommandExecutor commandExecutorTxRequiresNew;

	  /// <summary>
	  /// Separate command executor to be used for db schema operations. Must always use NON-JTA transactions
	  /// </summary>
	  protected internal CommandExecutor commandExecutorSchemaOperations;

	  // SESSION FACTORIES ////////////////////////////////////////////////////////

	  protected internal IList<SessionFactory> customSessionFactories;
	  protected internal DbSqlSessionFactory dbSqlSessionFactory;
	  protected internal IDictionary<Type, SessionFactory> sessionFactories;

	  // DEPLOYERS ////////////////////////////////////////////////////////////////

	  protected internal IList<Deployer> customPreDeployers;
	  protected internal IList<Deployer> customPostDeployers;
	  protected internal IList<Deployer> deployers;
	  protected internal DeploymentCache deploymentCache;

	  // CACHE ////////////////////////////////////////////////////////////////////

	  protected internal CacheFactory cacheFactory;
	  protected internal int cacheCapacity = 1000;
	  protected internal bool enableFetchProcessDefinitionDescription = true;

	  // JOB EXECUTOR /////////////////////////////////////////////////////////////

	  protected internal IList<JobHandler> customJobHandlers;
	  protected internal IDictionary<string, JobHandler> jobHandlers;
	  protected internal JobExecutor jobExecutor;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.impl.PriorityProvider<org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration<?, ?>> jobPriorityProvider;
	  protected internal PriorityProvider<JobDeclaration<object, ?>> jobPriorityProvider;

	  // EXTERNAL TASK /////////////////////////////////////////////////////////////
	  protected internal PriorityProvider<ExternalTaskActivityBehavior> externalTaskPriorityProvider;

	  // MYBATIS SQL SESSION FACTORY //////////////////////////////////////////////

	  protected internal SqlSessionFactory sqlSessionFactory;
	  protected internal TransactionFactory transactionFactory;


	  // ID GENERATOR /////////////////////////////////////////////////////////////
	  protected internal IdGenerator idGenerator;
	  protected internal DataSource idGeneratorDataSource;
	  protected internal string idGeneratorDataSourceJndiName;

	  // INCIDENT HANDLER /////////////////////////////////////////////////////////

	  protected internal IDictionary<string, IncidentHandler> incidentHandlers;
	  protected internal IList<IncidentHandler> customIncidentHandlers;

	  // BATCH ////////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Map<String, org.camunda.bpm.engine.impl.batch.BatchJobHandler<?>> batchHandlers;
	  protected internal IDictionary<string, BatchJobHandler<object>> batchHandlers;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<org.camunda.bpm.engine.impl.batch.BatchJobHandler<?>> customBatchJobHandlers;
	  protected internal IList<BatchJobHandler<object>> customBatchJobHandlers;

	  /// <summary>
	  /// Number of jobs created by a batch seed job invocation
	  /// </summary>
	  protected internal int batchJobsPerSeed = 100;
	  /// <summary>
	  /// Number of invocations executed by a single batch job
	  /// </summary>
	  protected internal int invocationsPerBatchJob = 1;
	  /// <summary>
	  /// seconds to wait between polling for batch completion
	  /// </summary>
	  protected internal int batchPollTime = 30;
	  /// <summary>
	  /// default priority for batch jobs
	  /// </summary>
	  protected internal long batchJobPriority = DefaultJobPriorityProvider.DEFAULT_PRIORITY;

	  // OTHER ////////////////////////////////////////////////////////////////////
	  protected internal IList<FormEngine> customFormEngines;
	  protected internal IDictionary<string, FormEngine> formEngines;

	  protected internal IList<AbstractFormFieldType> customFormTypes;
	  protected internal FormTypes formTypes;
	  protected internal FormValidators formValidators;
	  protected internal IDictionary<string, Type> customFormFieldValidators;

	  protected internal IList<TypedValueSerializer> customPreVariableSerializers;
	  protected internal IList<TypedValueSerializer> customPostVariableSerializers;
	  protected internal VariableSerializers variableSerializers;
	  protected internal VariableSerializerFactory fallbackSerializerFactory;

	  protected internal string defaultSerializationFormat = Variables.SerializationDataFormats.JAVA.Name;
	  protected internal bool javaSerializationFormatEnabled = false;
	  protected internal string defaultCharsetName = null;
	  protected internal Charset defaultCharset = null;

	  protected internal ExpressionManager expressionManager;
	  protected internal ScriptingEngines scriptingEngines;
	  protected internal IList<ResolverFactory> resolverFactories;
	  protected internal ScriptingEnvironment scriptingEnvironment;
	  protected internal IList<ScriptEnvResolver> scriptEnvResolvers;
	  protected internal ScriptFactory scriptFactory;
	  protected internal bool autoStoreScriptVariables = false;
	  protected internal bool enableScriptCompilation = true;
	  protected internal bool enableScriptEngineCaching = true;
	  protected internal bool enableFetchScriptEngineFromProcessApplication = true;

	  protected internal bool cmmnEnabled = true;
	  protected internal bool dmnEnabled = true;

	  protected internal bool enableGracefulDegradationOnContextSwitchFailure = true;

	  protected internal BusinessCalendarManager businessCalendarManager;

	  protected internal string wsSyncFactoryClassName = DEFAULT_WS_SYNC_FACTORY;

	  protected internal CommandContextFactory commandContextFactory;
	  protected internal TransactionContextFactory transactionContextFactory;
	  protected internal BpmnParseFactory bpmnParseFactory;

	  // cmmn
	  protected internal CmmnTransformFactory cmmnTransformFactory;
	  protected internal DefaultCmmnElementHandlerRegistry cmmnElementHandlerRegistry;

	  // dmn
	  protected internal DefaultDmnEngineConfiguration dmnEngineConfiguration;
	  protected internal DmnEngine dmnEngine;

	  protected internal HistoryLevel historyLevel;

	  /// <summary>
	  /// a list of supported history levels
	  /// </summary>
	  protected internal IList<HistoryLevel> historyLevels;

	  /// <summary>
	  /// a list of supported custom history levels
	  /// </summary>
	  protected internal IList<HistoryLevel> customHistoryLevels;

	  protected internal IList<BpmnParseListener> preParseListeners;
	  protected internal IList<BpmnParseListener> postParseListeners;

	  protected internal IList<CmmnTransformListener> customPreCmmnTransformListeners;
	  protected internal IList<CmmnTransformListener> customPostCmmnTransformListeners;

	  protected internal IDictionary<object, object> beans;

	  protected internal bool isDbIdentityUsed = true;
	  protected internal bool isDbHistoryUsed = true;

	  protected internal DelegateInterceptor delegateInterceptor;

	  protected internal CommandInterceptor actualCommandExecutor;

	  protected internal RejectedJobsHandler customRejectedJobsHandler;

	  protected internal IDictionary<string, EventHandler> eventHandlers;
	  protected internal IList<EventHandler> customEventHandlers;

	  protected internal FailedJobCommandFactory failedJobCommandFactory;

	  protected internal string databaseTablePrefix = "";

	  /// <summary>
	  /// In some situations you want to set the schema to use for table checks / generation if the database metadata
	  /// doesn't return that correctly, see https://jira.codehaus.org/browse/ACT-1220,
	  /// https://jira.codehaus.org/browse/ACT-1062
	  /// </summary>
	  protected internal string databaseSchema = null;

	  protected internal bool isCreateDiagramOnDeploy = false;

	  protected internal ProcessApplicationManager processApplicationManager;

	  protected internal CorrelationHandler correlationHandler;

	  protected internal ConditionHandler conditionHandler;

	  /// <summary>
	  /// session factory to be used for obtaining identity provider sessions
	  /// </summary>
	  protected internal SessionFactory identityProviderSessionFactory;

	  protected internal PasswordEncryptor passwordEncryptor;

	  protected internal IList<PasswordEncryptor> customPasswordChecker;

	  protected internal PasswordManager passwordManager;

	  protected internal SaltGenerator saltGenerator;

	  protected internal ISet<string> registeredDeployments;

	  protected internal ResourceAuthorizationProvider resourceAuthorizationProvider;

	  protected internal IList<ProcessEnginePlugin> processEnginePlugins = new List<ProcessEnginePlugin>();

	  protected internal HistoryEventProducer historyEventProducer;

	  protected internal CmmnHistoryEventProducer cmmnHistoryEventProducer;

	  protected internal DmnHistoryEventProducer dmnHistoryEventProducer;

	  protected internal HistoryEventHandler historyEventHandler;

	  protected internal PermissionProvider permissionProvider;

	  protected internal bool isExecutionTreePrefetchEnabled = true;

	  /// <summary>
	  /// If true the process engine will attempt to acquire an exclusive lock before
	  /// creating a deployment.
	  /// </summary>
	  protected internal bool isDeploymentLockUsed = true;

	  /// <summary>
	  /// If true then several deployments will be processed strictly sequentally. When false they may be processed in parallel.
	  /// </summary>
	  protected internal bool isDeploymentSynchronized = true;

	  /// <summary>
	  /// Allows setting whether the process engine should try reusing the first level entity cache.
	  /// Default setting is false, enabling it improves performance of asynchronous continuations.
	  /// </summary>
	  protected internal bool isDbEntityCacheReuseEnabled = false;

	  protected internal bool isInvokeCustomVariableListeners = true;

	  /// <summary>
	  /// The process engine created by this configuration.
	  /// </summary>
	  protected internal ProcessEngineImpl processEngine;

	  /// <summary>
	  /// used to create instances for listeners, JavaDelegates, etc
	  /// </summary>
	  protected internal ArtifactFactory artifactFactory;

	  protected internal DbEntityCacheKeyMapping dbEntityCacheKeyMapping = DbEntityCacheKeyMapping.defaultEntityCacheKeyMapping();

	  /// <summary>
	  /// the metrics registry
	  /// </summary>
	  protected internal MetricsRegistry metricsRegistry;

	  protected internal DbMetricsReporter dbMetricsReporter;

	  protected internal bool isMetricsEnabled = true;
	  protected internal bool isDbMetricsReporterActivate = true;

	  protected internal MetricsReporterIdProvider metricsReporterIdProvider;

	  /// <summary>
	  /// handling of expressions submitted via API; can be used as guards against remote code execution
	  /// </summary>
	  protected internal bool enableExpressionsInAdhocQueries = false;
	  protected internal bool enableExpressionsInStoredQueries = true;

	  /// <summary>
	  /// If false, disables XML eXternal Entity (XXE) Processing. This provides protection against XXE Processing attacks.
	  /// </summary>
	  protected internal bool enableXxeProcessing = false;

	  /// <summary>
	  /// If true, user operation log entries are only written if there is an
	  /// authenticated user present in the context. If false, user operation log
	  /// entries are written regardless of authentication state.
	  /// </summary>
	  protected internal bool restrictUserOperationLogToAuthenticatedUsers = true;

	  protected internal bool disableStrictCallActivityValidation = false;

	  protected internal bool isBpmnStacktraceVerbose = false;

	  protected internal bool forceCloseMybatisConnectionPool = true;

	  protected internal TenantIdProvider tenantIdProvider = null;

	  protected internal IList<CommandChecker> commandCheckers = null;

	  protected internal IList<string> adminGroups;

	  protected internal IList<string> adminUsers;

	  // Migration
	  protected internal MigrationActivityMatcher migrationActivityMatcher;

	  protected internal IList<MigrationActivityValidator> customPreMigrationActivityValidators;
	  protected internal IList<MigrationActivityValidator> customPostMigrationActivityValidators;
	  protected internal MigrationInstructionGenerator migrationInstructionGenerator;

	  protected internal IList<MigrationInstructionValidator> customPreMigrationInstructionValidators;
	  protected internal IList<MigrationInstructionValidator> customPostMigrationInstructionValidators;
	  protected internal IList<MigrationInstructionValidator> migrationInstructionValidators;

	  protected internal IList<MigratingActivityInstanceValidator> customPreMigratingActivityInstanceValidators;
	  protected internal IList<MigratingActivityInstanceValidator> customPostMigratingActivityInstanceValidators;
	  protected internal IList<MigratingActivityInstanceValidator> migratingActivityInstanceValidators;
	  protected internal IList<MigratingTransitionInstanceValidator> migratingTransitionInstanceValidators;
	  protected internal IList<MigratingCompensationInstanceValidator> migratingCompensationInstanceValidators;

	  // Default user permission for task
	  protected internal Permission defaultUserPermissionForTask;

	  protected internal bool isUseSharedSqlSessionFactory = false;

	  //History cleanup configuration
	  protected internal string historyCleanupBatchWindowStartTime;
	  protected internal string historyCleanupBatchWindowEndTime = "00:00";

	  protected internal DateTime historyCleanupBatchWindowStartTimeAsDate;
	  protected internal DateTime historyCleanupBatchWindowEndTimeAsDate;

	  protected internal IDictionary<int, BatchWindowConfiguration> historyCleanupBatchWindows = new Dictionary<int, BatchWindowConfiguration>();

	  //shortcuts for batch windows configuration available to be configured from XML
	  protected internal string mondayHistoryCleanupBatchWindowStartTime;
	  protected internal string mondayHistoryCleanupBatchWindowEndTime;
	  protected internal string tuesdayHistoryCleanupBatchWindowStartTime;
	  protected internal string tuesdayHistoryCleanupBatchWindowEndTime;
	  protected internal string wednesdayHistoryCleanupBatchWindowStartTime;
	  protected internal string wednesdayHistoryCleanupBatchWindowEndTime;
	  protected internal string thursdayHistoryCleanupBatchWindowStartTime;
	  protected internal string thursdayHistoryCleanupBatchWindowEndTime;
	  protected internal string fridayHistoryCleanupBatchWindowStartTime;
	  protected internal string fridayHistoryCleanupBatchWindowEndTime;
	  protected internal string saturdayHistoryCleanupBatchWindowStartTime;
	  protected internal string saturdayHistoryCleanupBatchWindowEndTime;
	  protected internal string sundayHistoryCleanupBatchWindowStartTime;
	  protected internal string sundayHistoryCleanupBatchWindowEndTime;


	  protected internal int historyCleanupDegreeOfParallelism = 1;

	  protected internal string historyTimeToLive;

	  protected internal string batchOperationHistoryTimeToLive;
	  protected internal IDictionary<string, string> batchOperationsForHistoryCleanup;
	  protected internal IDictionary<string, int> parsedBatchOperationsForHistoryCleanup;

	  protected internal BatchWindowManager batchWindowManager = new DefaultBatchWindowManager();

	  protected internal HistoryRemovalTimeProvider historyRemovalTimeProvider;

	  protected internal string historyRemovalTimeStrategy;

	  protected internal string historyCleanupStrategy;

	  /// <summary>
	  /// Size of batch in which history cleanup data will be deleted. <seealso cref="HistoryCleanupBatch.MAX_BATCH_SIZE"/> must be respected.
	  /// </summary>
	  private int historyCleanupBatchSize = 500;
	  /// <summary>
	  /// Indicates the minimal amount of data to trigger the history cleanup.
	  /// </summary>
	  private int historyCleanupBatchThreshold = 10;

	  private bool historyCleanupMetricsEnabled = true;

	  private int failedJobListenerMaxRetries = DEFAULT_FAILED_JOB_LISTENER_MAX_RETRIES;

	  protected internal string failedJobRetryTimeCycle;

	  // login attempts ///////////////////////////////////////////////////////
	  protected internal int loginMaxAttempts = 10;
	  protected internal int loginDelayFactor = 2;
	  protected internal int loginDelayMaxTime = 60;
	  protected internal int loginDelayBase = 3;

	  // buildProcessEngine ///////////////////////////////////////////////////////

	  public override ProcessEngine buildProcessEngine()
	  {
		init();
		processEngine = new ProcessEngineImpl(this);
		invokePostProcessEngineBuild(processEngine);
		return processEngine;
	  }

	  // init /////////////////////////////////////////////////////////////////////

	  protected internal virtual void init()
	  {
		invokePreInit();
		initDefaultCharset();
		initHistoryLevel();
		initHistoryEventProducer();
		initCmmnHistoryEventProducer();
		initDmnHistoryEventProducer();
		initHistoryEventHandler();
		initExpressionManager();
		initBeans();
		initArtifactFactory();
		initFormEngines();
		initFormTypes();
		initFormFieldValidators();
		initScripting();
		initDmnEngine();
		initBusinessCalendarManager();
		initCommandContextFactory();
		initTransactionContextFactory();
		initCommandExecutors();
		initServices();
		initIdGenerator();
		initFailedJobCommandFactory();
		initDeployers();
		initJobProvider();
		initExternalTaskPriorityProvider();
		initBatchHandlers();
		initJobExecutor();
		initDataSource();
		initTransactionFactory();
		initSqlSessionFactory();
		initIdentityProviderSessionFactory();
		initSessionFactories();
		initValueTypeResolver();
		initSerialization();
		initJpa();
		initDelegateInterceptor();
		initEventHandlers();
		initProcessApplicationManager();
		initCorrelationHandler();
		initConditionHandler();
		initIncidentHandlers();
		initPasswordDigest();
		initDeploymentRegistration();
		initResourceAuthorizationProvider();
		initPermissionProvider();
		initMetrics();
		initMigration();
		initCommandCheckers();
		initDefaultUserPermissionForTask();
		initHistoryRemovalTime();
		initHistoryCleanup();
		initAdminUser();
		initAdminGroups();
		initPasswordPolicy();
		invokePostInit();
	  }

	  public virtual void initHistoryRemovalTime()
	  {
		initHistoryRemovalTimeProvider();
		initHistoryRemovalTimeStrategy();
	  }

	  public virtual void initHistoryRemovalTimeStrategy()
	  {
		if (string.ReferenceEquals(historyRemovalTimeStrategy, null))
		{
		  historyRemovalTimeStrategy = HISTORY_REMOVAL_TIME_STRATEGY_END;
		}

		if (!HISTORY_REMOVAL_TIME_STRATEGY_START.Equals(historyRemovalTimeStrategy) && !HISTORY_REMOVAL_TIME_STRATEGY_END.Equals(historyRemovalTimeStrategy) && !HISTORY_REMOVAL_TIME_STRATEGY_NONE.Equals(historyRemovalTimeStrategy))
		{
		  throw LOG.invalidPropertyValue("historyRemovalTimeStrategy", historyRemovalTimeStrategy.ToString(), string.Format("history removal time strategy must be set to '{0}', '{1}' or '{2}'", HISTORY_REMOVAL_TIME_STRATEGY_START, HISTORY_REMOVAL_TIME_STRATEGY_END, HISTORY_REMOVAL_TIME_STRATEGY_NONE));
		}
	  }

	  public virtual void initHistoryRemovalTimeProvider()
	  {
		if (historyRemovalTimeProvider == null)
		{
		  historyRemovalTimeProvider = new DefaultHistoryRemovalTimeProvider();
		}
	  }

	  public virtual void initHistoryCleanup()
	  {
		initHistoryCleanupStrategy();

		//validate number of threads
		if (historyCleanupDegreeOfParallelism < 1 || historyCleanupDegreeOfParallelism > MAX_THREADS_NUMBER)
		{
		  throw LOG.invalidPropertyValue("historyCleanupDegreeOfParallelism", historyCleanupDegreeOfParallelism.ToString(), string.Format("value for number of threads for history cleanup should be between 1 and {0}", HistoryCleanupCmd.MAX_THREADS_NUMBER));
		}

		if (!string.ReferenceEquals(historyCleanupBatchWindowStartTime, null))
		{
		  initHistoryCleanupBatchWindowStartTime();
		}

		if (!string.ReferenceEquals(historyCleanupBatchWindowEndTime, null))
		{
		  initHistoryCleanupBatchWindowEndTime();
		}

		initHistoryCleanupBatchWindowsMap();

		if (historyCleanupBatchSize > HistoryCleanupHandler.MAX_BATCH_SIZE || historyCleanupBatchSize <= 0)
		{
		  throw LOG.invalidPropertyValue("historyCleanupBatchSize", historyCleanupBatchSize.ToString(), string.Format("value for batch size should be between 1 and {0}", HistoryCleanupHandler.MAX_BATCH_SIZE));
		}

		if (historyCleanupBatchThreshold < 0)
		{
		  throw LOG.invalidPropertyValue("historyCleanupBatchThreshold", historyCleanupBatchThreshold.ToString(), "History cleanup batch threshold cannot be negative.");
		}

		initHistoryTimeToLive();

		initBatchOperationsHistoryTimeToLive();
	  }

	  protected internal virtual void initHistoryCleanupStrategy()
	  {
		if (string.ReferenceEquals(historyCleanupStrategy, null))
		{
		  historyCleanupStrategy = HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED;
		}

		if (!HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED.Equals(historyCleanupStrategy) && !HISTORY_CLEANUP_STRATEGY_END_TIME_BASED.Equals(historyCleanupStrategy))
		{
		  throw LOG.invalidPropertyValue("historyCleanupStrategy", historyCleanupStrategy.ToString(), string.Format("history cleanup strategy must be either set to '{0}' or '{1}'", HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED, HISTORY_CLEANUP_STRATEGY_END_TIME_BASED));
		}

		if (HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED.Equals(historyCleanupStrategy) && HISTORY_REMOVAL_TIME_STRATEGY_NONE.Equals(historyRemovalTimeStrategy))
		{
		  throw LOG.invalidPropertyValue("historyRemovalTimeStrategy", historyRemovalTimeStrategy.ToString(), string.Format("history removal time strategy cannot be set to '{0}' in conjunction with '{1}' history cleanup strategy", HISTORY_REMOVAL_TIME_STRATEGY_NONE, HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED));
		}
	  }

	  private void initHistoryCleanupBatchWindowsMap()
	  {
		if (!string.ReferenceEquals(mondayHistoryCleanupBatchWindowStartTime, null) || !string.ReferenceEquals(mondayHistoryCleanupBatchWindowEndTime, null))
		{
		  historyCleanupBatchWindows[DayOfWeek.Monday] = new BatchWindowConfiguration(mondayHistoryCleanupBatchWindowStartTime, mondayHistoryCleanupBatchWindowEndTime);
		}

		if (!string.ReferenceEquals(tuesdayHistoryCleanupBatchWindowStartTime, null) || !string.ReferenceEquals(tuesdayHistoryCleanupBatchWindowEndTime, null))
		{
		  historyCleanupBatchWindows[DayOfWeek.Tuesday] = new BatchWindowConfiguration(tuesdayHistoryCleanupBatchWindowStartTime, tuesdayHistoryCleanupBatchWindowEndTime);
		}

		if (!string.ReferenceEquals(wednesdayHistoryCleanupBatchWindowStartTime, null) || !string.ReferenceEquals(wednesdayHistoryCleanupBatchWindowEndTime, null))
		{
		  historyCleanupBatchWindows[DayOfWeek.Wednesday] = new BatchWindowConfiguration(wednesdayHistoryCleanupBatchWindowStartTime, wednesdayHistoryCleanupBatchWindowEndTime);
		}

		if (!string.ReferenceEquals(thursdayHistoryCleanupBatchWindowStartTime, null) || !string.ReferenceEquals(thursdayHistoryCleanupBatchWindowEndTime, null))
		{
		  historyCleanupBatchWindows[DayOfWeek.Thursday] = new BatchWindowConfiguration(thursdayHistoryCleanupBatchWindowStartTime, thursdayHistoryCleanupBatchWindowEndTime);
		}

		if (!string.ReferenceEquals(fridayHistoryCleanupBatchWindowStartTime, null) || !string.ReferenceEquals(fridayHistoryCleanupBatchWindowEndTime, null))
		{
		  historyCleanupBatchWindows[DayOfWeek.Friday] = new BatchWindowConfiguration(fridayHistoryCleanupBatchWindowStartTime, fridayHistoryCleanupBatchWindowEndTime);
		}

		if (!string.ReferenceEquals(saturdayHistoryCleanupBatchWindowStartTime, null) || !string.ReferenceEquals(saturdayHistoryCleanupBatchWindowEndTime, null))
		{
		  historyCleanupBatchWindows[DayOfWeek.Saturday] = new BatchWindowConfiguration(saturdayHistoryCleanupBatchWindowStartTime, saturdayHistoryCleanupBatchWindowEndTime);
		}

		if (!string.ReferenceEquals(sundayHistoryCleanupBatchWindowStartTime, null) || !string.ReferenceEquals(sundayHistoryCleanupBatchWindowEndTime, null))
		{
		  historyCleanupBatchWindows[DayOfWeek.Sunday] = new BatchWindowConfiguration(sundayHistoryCleanupBatchWindowStartTime, sundayHistoryCleanupBatchWindowEndTime);
		}
	  }

	  protected internal virtual void initHistoryTimeToLive()
	  {
		try
		{
		  ParseUtil.parseHistoryTimeToLive(historyTimeToLive);
		}
		catch (Exception e)
		{
		  throw LOG.invalidPropertyValue("historyTimeToLive", historyTimeToLive, e);
		}
	  }

	  protected internal virtual void initBatchOperationsHistoryTimeToLive()
	  {
		try
		{
		  ParseUtil.parseHistoryTimeToLive(batchOperationHistoryTimeToLive);
		}
		catch (Exception e)
		{
		  throw LOG.invalidPropertyValue("batchOperationHistoryTimeToLive", batchOperationHistoryTimeToLive, e);
		}

		if (batchOperationsForHistoryCleanup == null)
		{
		  batchOperationsForHistoryCleanup = new Dictionary<string, string>();
		}
		else
		{
		  foreach (string batchOperation in batchOperationsForHistoryCleanup.Keys)
		  {
			string timeToLive = batchOperationsForHistoryCleanup[batchOperation];
			if (!batchHandlers.Keys.Contains(batchOperation))
			{
			  LOG.invalidBatchOperation(batchOperation, timeToLive);
			}

			try
			{
			  ParseUtil.parseHistoryTimeToLive(timeToLive);
			}
			catch (Exception e)
			{
			  throw LOG.invalidPropertyValue("history time to live for " + batchOperation + " batch operations", timeToLive, e);
			}
		  }
		}

		if (batchHandlers != null && !string.ReferenceEquals(batchOperationHistoryTimeToLive, null))
		{

		  foreach (string batchOperation in batchHandlers.Keys)
		  {
			if (!batchOperationsForHistoryCleanup.ContainsKey(batchOperation))
			{
			  batchOperationsForHistoryCleanup[batchOperation] = batchOperationHistoryTimeToLive;
			}
		  }
		}

		parsedBatchOperationsForHistoryCleanup = new Dictionary<string, int>();
		if (batchOperationsForHistoryCleanup != null)
		{
		  foreach (string operation in batchOperationsForHistoryCleanup.Keys)
		  {
			int? historyTimeToLive = ParseUtil.parseHistoryTimeToLive(batchOperationsForHistoryCleanup[operation]);
			parsedBatchOperationsForHistoryCleanup[operation] = historyTimeToLive.Value;
		  }
		}
	  }

	  private void initHistoryCleanupBatchWindowEndTime()
	  {
		try
		{
		  historyCleanupBatchWindowEndTimeAsDate = HistoryCleanupHelper.parseTimeConfiguration(historyCleanupBatchWindowEndTime);
		}
		catch (ParseException)
		{
		  throw LOG.invalidPropertyValue("historyCleanupBatchWindowEndTime", historyCleanupBatchWindowEndTime);
		}
	  }

	  private void initHistoryCleanupBatchWindowStartTime()
	  {
		try
		{
		  historyCleanupBatchWindowStartTimeAsDate = HistoryCleanupHelper.parseTimeConfiguration(historyCleanupBatchWindowStartTime);
		}
		catch (ParseException)
		{
		  throw LOG.invalidPropertyValue("historyCleanupBatchWindowStartTime", historyCleanupBatchWindowStartTime);
		}
	  }

	  protected internal virtual void invokePreInit()
	  {
		foreach (ProcessEnginePlugin plugin in processEnginePlugins)
		{
		  LOG.pluginActivated(plugin.ToString(), ProcessEngineName);
		  plugin.preInit(this);
		}
	  }

	  protected internal virtual void invokePostInit()
	  {
		foreach (ProcessEnginePlugin plugin in processEnginePlugins)
		{
		  plugin.postInit(this);
		}
	  }

	  protected internal virtual void invokePostProcessEngineBuild(ProcessEngine engine)
	  {
		foreach (ProcessEnginePlugin plugin in processEnginePlugins)
		{
		  plugin.postProcessEngineBuild(engine);
		}
	  }


	  // failedJobCommandFactory ////////////////////////////////////////////////////////

	  protected internal virtual void initFailedJobCommandFactory()
	  {
		if (failedJobCommandFactory == null)
		{
		  failedJobCommandFactory = new DefaultFailedJobCommandFactory();
		}
		if (postParseListeners == null)
		{
		  postParseListeners = new List<BpmnParseListener>();
		}
		postParseListeners.Add(new DefaultFailedJobParseListener());

	  }

	  // incident handlers /////////////////////////////////////////////////////////////

	  protected internal virtual void initIncidentHandlers()
	  {
		if (incidentHandlers == null)
		{
		  incidentHandlers = new Dictionary<string, IncidentHandler>();

		  DefaultIncidentHandler failedJobIncidentHandler = new DefaultIncidentHandler(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE);
		  incidentHandlers[failedJobIncidentHandler.IncidentHandlerType] = failedJobIncidentHandler;

		  DefaultIncidentHandler failedExternalTaskIncidentHandler = new DefaultIncidentHandler(org.camunda.bpm.engine.runtime.Incident_Fields.EXTERNAL_TASK_HANDLER_TYPE);
		  incidentHandlers[failedExternalTaskIncidentHandler.IncidentHandlerType] = failedExternalTaskIncidentHandler;
		}
		if (customIncidentHandlers != null)
		{
		  foreach (IncidentHandler incidentHandler in customIncidentHandlers)
		  {
			incidentHandlers[incidentHandler.IncidentHandlerType] = incidentHandler;
		  }
		}
	  }

	  // batch ///////////////////////////////////////////////////////////////////////

	  protected internal virtual void initBatchHandlers()
	  {
		if (batchHandlers == null)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: batchHandlers = new java.util.HashMap<String, org.camunda.bpm.engine.impl.batch.BatchJobHandler<?>>();
		  batchHandlers = new Dictionary<string, BatchJobHandler<object>>();

		  MigrationBatchJobHandler migrationHandler = new MigrationBatchJobHandler();
		  batchHandlers[migrationHandler.Type] = migrationHandler;

		  ModificationBatchJobHandler modificationHandler = new ModificationBatchJobHandler();
		  batchHandlers[modificationHandler.Type] = modificationHandler;

		  DeleteProcessInstancesJobHandler deleteProcessJobHandler = new DeleteProcessInstancesJobHandler();
		  batchHandlers[deleteProcessJobHandler.Type] = deleteProcessJobHandler;

		  DeleteHistoricProcessInstancesJobHandler deleteHistoricProcessInstancesJobHandler = new DeleteHistoricProcessInstancesJobHandler();
		  batchHandlers[deleteHistoricProcessInstancesJobHandler.Type] = deleteHistoricProcessInstancesJobHandler;

		  SetJobRetriesJobHandler setJobRetriesJobHandler = new SetJobRetriesJobHandler();
		  batchHandlers[setJobRetriesJobHandler.Type] = setJobRetriesJobHandler;

		  SetExternalTaskRetriesJobHandler setExternalTaskRetriesJobHandler = new SetExternalTaskRetriesJobHandler();
		  batchHandlers[setExternalTaskRetriesJobHandler.Type] = setExternalTaskRetriesJobHandler;

		  RestartProcessInstancesJobHandler restartProcessInstancesJobHandler = new RestartProcessInstancesJobHandler();
		  batchHandlers[restartProcessInstancesJobHandler.Type] = restartProcessInstancesJobHandler;

		  UpdateProcessInstancesSuspendStateJobHandler suspendProcessInstancesJobHandler = new UpdateProcessInstancesSuspendStateJobHandler();
		  batchHandlers[suspendProcessInstancesJobHandler.Type] = suspendProcessInstancesJobHandler;

		  DeleteHistoricDecisionInstancesJobHandler deleteHistoricDecisionInstancesJobHandler = new DeleteHistoricDecisionInstancesJobHandler();
		  batchHandlers[deleteHistoricDecisionInstancesJobHandler.Type] = deleteHistoricDecisionInstancesJobHandler;

		  ProcessSetRemovalTimeJobHandler processSetRemovalTimeJobHandler = new ProcessSetRemovalTimeJobHandler();
		  batchHandlers[processSetRemovalTimeJobHandler.Type] = processSetRemovalTimeJobHandler;

		  DecisionSetRemovalTimeJobHandler decisionSetRemovalTimeJobHandler = new DecisionSetRemovalTimeJobHandler();
		  batchHandlers[decisionSetRemovalTimeJobHandler.Type] = decisionSetRemovalTimeJobHandler;

		  BatchSetRemovalTimeJobHandler batchSetRemovalTimeJobHandler = new BatchSetRemovalTimeJobHandler();
		  batchHandlers[batchSetRemovalTimeJobHandler.Type] = batchSetRemovalTimeJobHandler;
		}

		if (customBatchJobHandlers != null)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.engine.impl.batch.BatchJobHandler<?> customBatchJobHandler : customBatchJobHandlers)
		  foreach (BatchJobHandler<object> customBatchJobHandler in customBatchJobHandlers)
		  {
			batchHandlers[customBatchJobHandler.Type] = customBatchJobHandler;
		  }
		}
	  }

	  // command executors ////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected abstract java.util.Collection<? extends org.camunda.bpm.engine.impl.interceptor.CommandInterceptor> getDefaultCommandInterceptorsTxRequired();
	  protected internal abstract ICollection<CommandInterceptor> DefaultCommandInterceptorsTxRequired {get;}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected abstract java.util.Collection<? extends org.camunda.bpm.engine.impl.interceptor.CommandInterceptor> getDefaultCommandInterceptorsTxRequiresNew();
	  protected internal abstract ICollection<CommandInterceptor> DefaultCommandInterceptorsTxRequiresNew {get;}

	  protected internal virtual void initCommandExecutors()
	  {
		initActualCommandExecutor();
		initCommandInterceptorsTxRequired();
		initCommandExecutorTxRequired();
		initCommandInterceptorsTxRequiresNew();
		initCommandExecutorTxRequiresNew();
		initCommandExecutorDbSchemaOperations();
	  }

	  protected internal virtual void initActualCommandExecutor()
	  {
		actualCommandExecutor = new CommandExecutorImpl();
	  }

	  protected internal virtual void initCommandInterceptorsTxRequired()
	  {
		if (commandInterceptorsTxRequired == null)
		{
		  if (customPreCommandInterceptorsTxRequired != null)
		  {
			commandInterceptorsTxRequired = new List<CommandInterceptor>(customPreCommandInterceptorsTxRequired);
		  }
		  else
		  {
			commandInterceptorsTxRequired = new List<CommandInterceptor>();
		  }
		  ((IList<CommandInterceptor>)commandInterceptorsTxRequired).AddRange(DefaultCommandInterceptorsTxRequired);
		  if (customPostCommandInterceptorsTxRequired != null)
		  {
			((IList<CommandInterceptor>)commandInterceptorsTxRequired).AddRange(customPostCommandInterceptorsTxRequired);
		  }
		  commandInterceptorsTxRequired.Add(actualCommandExecutor);
		}
	  }

	  protected internal virtual void initCommandInterceptorsTxRequiresNew()
	  {
		if (commandInterceptorsTxRequiresNew == null)
		{
		  if (customPreCommandInterceptorsTxRequiresNew != null)
		  {
			commandInterceptorsTxRequiresNew = new List<CommandInterceptor>(customPreCommandInterceptorsTxRequiresNew);
		  }
		  else
		  {
			commandInterceptorsTxRequiresNew = new List<CommandInterceptor>();
		  }
		  ((IList<CommandInterceptor>)commandInterceptorsTxRequiresNew).AddRange(DefaultCommandInterceptorsTxRequiresNew);
		  if (customPostCommandInterceptorsTxRequiresNew != null)
		  {
			((IList<CommandInterceptor>)commandInterceptorsTxRequiresNew).AddRange(customPostCommandInterceptorsTxRequiresNew);
		  }
		  commandInterceptorsTxRequiresNew.Add(actualCommandExecutor);
		}
	  }

	  protected internal virtual void initCommandExecutorTxRequired()
	  {
		if (commandExecutorTxRequired == null)
		{
		  commandExecutorTxRequired = initInterceptorChain(commandInterceptorsTxRequired);
		}
	  }

	  protected internal virtual void initCommandExecutorTxRequiresNew()
	  {
		if (commandExecutorTxRequiresNew == null)
		{
		  commandExecutorTxRequiresNew = initInterceptorChain(commandInterceptorsTxRequiresNew);
		}
	  }

	  protected internal virtual void initCommandExecutorDbSchemaOperations()
	  {
		if (commandExecutorSchemaOperations == null)
		{
		  // in default case, we use the same command executor for DB Schema Operations as for runtime operations.
		  // configurations that Use JTA Transactions should override this method and provide a custom command executor
		  // that uses NON-JTA Transactions.
		  commandExecutorSchemaOperations = commandExecutorTxRequired;
		}
	  }

	  protected internal virtual CommandInterceptor initInterceptorChain(IList<CommandInterceptor> chain)
	  {
		if (chain == null || chain.Count == 0)
		{
		  throw new ProcessEngineException("invalid command interceptor chain configuration: " + chain);
		}
		for (int i = 0; i < chain.Count - 1; i++)
		{
		  chain[i].Next = chain[i + 1];
		}
		return chain[0];
	  }

	  // services /////////////////////////////////////////////////////////////////

	  protected internal virtual void initServices()
	  {
		initService(repositoryService);
		initService(runtimeService);
		initService(historyService);
		initService(identityService);
		initService(taskService);
		initService(formService);
		initService(managementService);
		initService(authorizationService);
		initService(caseService);
		initService(filterService);
		initService(externalTaskService);
		initService(decisionService);
		initService(optimizeService);
	  }

	  protected internal virtual void initService(object service)
	  {
		if (service is ServiceImpl)
		{
		  ((ServiceImpl) service).CommandExecutor = commandExecutorTxRequired;
		}
		if (service is RepositoryServiceImpl)
		{
		  ((RepositoryServiceImpl) service).DeploymentCharset = DefaultCharset;
		}
	  }

	  // DataSource ///////////////////////////////////////////////////////////////

	  protected internal virtual void initDataSource()
	  {
		if (dataSource == null)
		{
		  if (!string.ReferenceEquals(dataSourceJndiName, null))
		  {
			try
			{
			  dataSource = (DataSource) (new InitialContext()).lookup(dataSourceJndiName);
			}
			catch (Exception e)
			{
			  throw new ProcessEngineException("couldn't lookup datasource from " + dataSourceJndiName + ": " + e.Message, e);
			}

		  }
		  else if (!string.ReferenceEquals(jdbcUrl, null))
		  {
			if ((string.ReferenceEquals(jdbcDriver, null)) || (string.ReferenceEquals(jdbcUrl, null)) || (string.ReferenceEquals(jdbcUsername, null)))
			{
			  throw new ProcessEngineException("DataSource or JDBC properties have to be specified in a process engine configuration");
			}

			PooledDataSource pooledDataSource = new PooledDataSource(ReflectUtil.ClassLoader, jdbcDriver, jdbcUrl, jdbcUsername, jdbcPassword);

			if (jdbcMaxActiveConnections > 0)
			{
			  pooledDataSource.PoolMaximumActiveConnections = jdbcMaxActiveConnections;
			}
			if (jdbcMaxIdleConnections > 0)
			{
			  pooledDataSource.PoolMaximumIdleConnections = jdbcMaxIdleConnections;
			}
			if (jdbcMaxCheckoutTime > 0)
			{
			  pooledDataSource.PoolMaximumCheckoutTime = jdbcMaxCheckoutTime;
			}
			if (jdbcMaxWaitTime > 0)
			{
			  pooledDataSource.PoolTimeToWait = jdbcMaxWaitTime;
			}
			if (jdbcPingEnabled == true)
			{
			  pooledDataSource.PoolPingEnabled = true;
			  if (!string.ReferenceEquals(jdbcPingQuery, null))
			  {
				pooledDataSource.PoolPingQuery = jdbcPingQuery;
			  }
			  pooledDataSource.PoolPingConnectionsNotUsedFor = jdbcPingConnectionNotUsedFor;
			}
			dataSource = pooledDataSource;
		  }

		  if (dataSource is PooledDataSource)
		  {
			// ACT-233: connection pool of Ibatis is not properely initialized if this is not called!
			((PooledDataSource) dataSource).forceCloseAll();
		  }
		}

		if (string.ReferenceEquals(databaseType, null))
		{
		  initDatabaseType();
		}
	  }

	  protected internal static Properties databaseTypeMappings = DefaultDatabaseTypeMappings;
	  protected internal const string MY_SQL_PRODUCT_NAME = "MySQL";
	  protected internal const string MARIA_DB_PRODUCT_NAME = "MariaDB";

	  protected internal static Properties DefaultDatabaseTypeMappings
	  {
		  get
		  {
			Properties databaseTypeMappings = new Properties();
			databaseTypeMappings.setProperty("H2", "h2");
			databaseTypeMappings.setProperty(MY_SQL_PRODUCT_NAME, "mysql");
			databaseTypeMappings.setProperty(MARIA_DB_PRODUCT_NAME, "mariadb");
			databaseTypeMappings.setProperty("Oracle", "oracle");
			databaseTypeMappings.setProperty("PostgreSQL", "postgres");
			databaseTypeMappings.setProperty("Microsoft SQL Server", "mssql");
			databaseTypeMappings.setProperty("DB2", "db2");
			databaseTypeMappings.setProperty("DB2", "db2");
			databaseTypeMappings.setProperty("DB2/NT", "db2");
			databaseTypeMappings.setProperty("DB2/NT64", "db2");
			databaseTypeMappings.setProperty("DB2 UDP", "db2");
			databaseTypeMappings.setProperty("DB2/LINUX", "db2");
			databaseTypeMappings.setProperty("DB2/LINUX390", "db2");
			databaseTypeMappings.setProperty("DB2/LINUXX8664", "db2");
			databaseTypeMappings.setProperty("DB2/LINUXZ64", "db2");
			databaseTypeMappings.setProperty("DB2/400 SQL", "db2");
			databaseTypeMappings.setProperty("DB2/6000", "db2");
			databaseTypeMappings.setProperty("DB2 UDB iSeries", "db2");
			databaseTypeMappings.setProperty("DB2/AIX64", "db2");
			databaseTypeMappings.setProperty("DB2/HPUX", "db2");
			databaseTypeMappings.setProperty("DB2/HP64", "db2");
			databaseTypeMappings.setProperty("DB2/SUN", "db2");
			databaseTypeMappings.setProperty("DB2/SUN64", "db2");
			databaseTypeMappings.setProperty("DB2/PTX", "db2");
			databaseTypeMappings.setProperty("DB2/2", "db2");
			return databaseTypeMappings;
		  }
	  }

	  public virtual void initDatabaseType()
	  {
		Connection connection = null;
		try
		{
		  connection = dataSource.Connection;
		  DatabaseMetaData databaseMetaData = connection.MetaData;
		  string databaseProductName = databaseMetaData.DatabaseProductName;
		  if (MY_SQL_PRODUCT_NAME.Equals(databaseProductName))
		  {
			databaseProductName = checkForMariaDb(databaseMetaData, databaseProductName);
		  }
		  LOG.debugDatabaseproductName(databaseProductName);
		  databaseType = databaseTypeMappings.getProperty(databaseProductName);
		  ensureNotNull("couldn't deduct database type from database product name '" + databaseProductName + "'", "databaseType", databaseType);
		  LOG.debugDatabaseType(databaseType);

		}
		catch (SQLException e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		}
		finally
		{
		  try
		  {
			if (connection != null)
			{
			  connection.close();
			}
		  }
		  catch (SQLException e)
		  {
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
		  }
		}
	  }

	  /// <summary>
	  /// The product name of mariadb is still 'MySQL'. This method
	  /// tries if it can find some evidence for mariadb. If it is successful
	  /// it will return "MariaDB", otherwise the provided database name.
	  /// </summary>
	  protected internal virtual string checkForMariaDb(DatabaseMetaData databaseMetaData, string databaseName)
	  {
		try
		{
		  string databaseProductVersion = databaseMetaData.DatabaseProductVersion;
		  if (!string.ReferenceEquals(databaseProductVersion, null) && databaseProductVersion.ToLower().Contains("mariadb"))
		  {
			return MARIA_DB_PRODUCT_NAME;
		  }
		}
		catch (SQLException)
		{
		}

		try
		{
		  string driverName = databaseMetaData.DriverName;
		  if (!string.ReferenceEquals(driverName, null) && driverName.ToLower().Contains("mariadb"))
		  {
			return MARIA_DB_PRODUCT_NAME;
		  }
		}
		catch (SQLException)
		{
		}

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		string metaDataClassName = databaseMetaData.GetType().FullName;
		if (!string.ReferenceEquals(metaDataClassName, null) && metaDataClassName.ToLower().Contains("mariadb"))
		{
		  return MARIA_DB_PRODUCT_NAME;
		}

		return databaseName;
	  }

	  // myBatis SqlSessionFactory ////////////////////////////////////////////////

	  protected internal virtual void initTransactionFactory()
	  {
		if (transactionFactory == null)
		{
		  if (transactionsExternallyManaged)
		  {
			transactionFactory = new ManagedTransactionFactory();
		  }
		  else
		  {
			transactionFactory = new JdbcTransactionFactory();
		  }
		}
	  }

	  protected internal virtual void initSqlSessionFactory()
	  {

		// to protect access to cachedSqlSessionFactory see CAM-6682
		lock (typeof(ProcessEngineConfigurationImpl))
		{

		  if (isUseSharedSqlSessionFactory)
		  {
			sqlSessionFactory = cachedSqlSessionFactory;
		  }

		  if (sqlSessionFactory == null)
		  {
			Stream inputStream = null;
			try
			{
			  inputStream = MyBatisXmlConfigurationSteam;

			  // update the jdbc parameters to the configured ones...
			  Environment environment = new Environment("default", transactionFactory, dataSource);
			  Reader reader = new StreamReader(inputStream);

			  Properties properties = new Properties();

			  if (isUseSharedSqlSessionFactory)
			  {
				properties.put("prefix", "${@org.camunda.bpm.engine.impl.context.Context@getProcessEngineConfiguration().databaseTablePrefix}");
			  }
			  else
			  {
				properties.put("prefix", databaseTablePrefix);
			  }

			  initSqlSessionFactoryProperties(properties, databaseTablePrefix, databaseType);

			  XMLConfigBuilder parser = new XMLConfigBuilder(reader, "", properties);
			  Configuration configuration = parser.Configuration;
			  configuration.Environment = environment;
			  configuration = parser.parse();

			  configuration.DefaultStatementTimeout = jdbcStatementTimeout;

			  if (JdbcBatchProcessing)
			  {
				configuration.DefaultExecutorType = ExecutorType.BATCH;
			  }

			  sqlSessionFactory = new DefaultSqlSessionFactory(configuration);

			  if (isUseSharedSqlSessionFactory)
			  {
				cachedSqlSessionFactory = sqlSessionFactory;
			  }


			}
			catch (Exception e)
			{
			  throw new ProcessEngineException("Error while building ibatis SqlSessionFactory: " + e.Message, e);
			}
			finally
			{
			  IoUtil.closeSilently(inputStream);
			}
		  }
		}
	  }

	  public static void initSqlSessionFactoryProperties(Properties properties, string databaseTablePrefix, string databaseType)
	  {

		if (!string.ReferenceEquals(databaseType, null))
		{
		  properties.put("limitBefore", DbSqlSessionFactory.databaseSpecificLimitBeforeStatements[databaseType]);
		  properties.put("limitAfter", DbSqlSessionFactory.databaseSpecificLimitAfterStatements[databaseType]);
		  properties.put("limitBeforeWithoutOffset", DbSqlSessionFactory.databaseSpecificLimitBeforeWithoutOffsetStatements[databaseType]);
		  properties.put("limitAfterWithoutOffset", DbSqlSessionFactory.databaseSpecificLimitAfterWithoutOffsetStatements[databaseType]);

		  properties.put("optimizeLimitBeforeWithoutOffset", DbSqlSessionFactory.optimizeDatabaseSpecificLimitBeforeWithoutOffsetStatements[databaseType]);
		  properties.put("optimizeLimitAfterWithoutOffset", DbSqlSessionFactory.optimizeDatabaseSpecificLimitAfterWithoutOffsetStatements[databaseType]);
		  properties.put("innerLimitAfter", DbSqlSessionFactory.databaseSpecificInnerLimitAfterStatements[databaseType]);
		  properties.put("limitBetween", DbSqlSessionFactory.databaseSpecificLimitBetweenStatements[databaseType]);
		  properties.put("limitBetweenFilter", DbSqlSessionFactory.databaseSpecificLimitBetweenFilterStatements[databaseType]);
		  properties.put("orderBy", DbSqlSessionFactory.databaseSpecificOrderByStatements[databaseType]);
		  properties.put("limitBeforeNativeQuery", DbSqlSessionFactory.databaseSpecificLimitBeforeNativeQueryStatements[databaseType]);
		  properties.put("distinct", DbSqlSessionFactory.databaseSpecificDistinct[databaseType]);

		  properties.put("escapeChar", DbSqlSessionFactory.databaseSpecificEscapeChar[databaseType]);

		  properties.put("bitand1", DbSqlSessionFactory.databaseSpecificBitAnd1[databaseType]);
		  properties.put("bitand2", DbSqlSessionFactory.databaseSpecificBitAnd2[databaseType]);
		  properties.put("bitand3", DbSqlSessionFactory.databaseSpecificBitAnd3[databaseType]);

		  properties.put("datepart1", DbSqlSessionFactory.databaseSpecificDatepart1[databaseType]);
		  properties.put("datepart2", DbSqlSessionFactory.databaseSpecificDatepart2[databaseType]);
		  properties.put("datepart3", DbSqlSessionFactory.databaseSpecificDatepart3[databaseType]);

		  properties.put("trueConstant", DbSqlSessionFactory.databaseSpecificTrueConstant[databaseType]);
		  properties.put("falseConstant", DbSqlSessionFactory.databaseSpecificFalseConstant[databaseType]);

		  properties.put("dbSpecificDummyTable", DbSqlSessionFactory.databaseSpecificDummyTable[databaseType]);
		  properties.put("dbSpecificIfNullFunction", DbSqlSessionFactory.databaseSpecificIfNull[databaseType]);

		  properties.put("dayComparator", DbSqlSessionFactory.databaseSpecificDaysComparator[databaseType]);

		  properties.put("collationForCaseSensitivity", DbSqlSessionFactory.databaseSpecificCollationForCaseSensitivity[databaseType]);

		  IDictionary<string, string> constants = DbSqlSessionFactory.dbSpecificConstants[databaseType];
		  foreach (KeyValuePair<string, string> entry in constants.SetOfKeyValuePairs())
		  {
			properties.put(entry.Key, entry.Value);
		  }
		}
	  }

	  protected internal virtual Stream MyBatisXmlConfigurationSteam
	  {
		  get
		  {
			return ReflectUtil.getResourceAsStream(DEFAULT_MYBATIS_MAPPING_FILE);
		  }
	  }

	  // session factories ////////////////////////////////////////////////////////

	  protected internal virtual void initIdentityProviderSessionFactory()
	  {
		if (identityProviderSessionFactory == null)
		{
		  identityProviderSessionFactory = new GenericManagerFactory(typeof(DbIdentityServiceProvider));
		}
	  }

	  protected internal virtual void initSessionFactories()
	  {
		if (sessionFactories == null)
		{
		  sessionFactories = new Dictionary<Type, SessionFactory>();

		  initPersistenceProviders();

		  addSessionFactory(new DbEntityManagerFactory(idGenerator));

		  addSessionFactory(new GenericManagerFactory(typeof(AttachmentManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(CommentManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(DeploymentManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(ExecutionManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricActivityInstanceManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricCaseActivityInstanceManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricStatisticsManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricDetailManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricProcessInstanceManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricCaseInstanceManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(UserOperationLogManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricTaskInstanceManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricVariableInstanceManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricIncidentManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricIdentityLinkLogManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricJobLogManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricExternalTaskLogManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(IdentityInfoManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(IdentityLinkManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(JobManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(JobDefinitionManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(ProcessDefinitionManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(PropertyManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(ResourceManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(ByteArrayManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(TableDataManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(TaskManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(TaskReportManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(VariableInstanceManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(EventSubscriptionManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(StatisticsManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(IncidentManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(AuthorizationManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(FilterManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(MeterLogManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(ExternalTaskManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(ReportManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(BatchManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricBatchManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(TenantManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(SchemaLogManager)));

		  addSessionFactory(new GenericManagerFactory(typeof(CaseDefinitionManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(CaseExecutionManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(CaseSentryPartManager)));

		  addSessionFactory(new GenericManagerFactory(typeof(DecisionDefinitionManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(DecisionRequirementsDefinitionManager)));
		  addSessionFactory(new GenericManagerFactory(typeof(HistoricDecisionInstanceManager)));

		  addSessionFactory(new GenericManagerFactory(typeof(OptimizeManager)));

		  sessionFactories[typeof(ReadOnlyIdentityProvider)] = identityProviderSessionFactory;

		  // check whether identityProviderSessionFactory implements WritableIdentityProvider
		  Type identityProviderType = identityProviderSessionFactory.SessionType;
		  if (identityProviderType.IsAssignableFrom(typeof(WritableIdentityProvider)))
		  {
			sessionFactories[typeof(WritableIdentityProvider)] = identityProviderSessionFactory;
		  }

		}
		if (customSessionFactories != null)
		{
		  foreach (SessionFactory sessionFactory in customSessionFactories)
		  {
			addSessionFactory(sessionFactory);
		  }
		}
	  }

	  protected internal virtual void initPersistenceProviders()
	  {
		ensurePrefixAndSchemaFitToegether(databaseTablePrefix, databaseSchema);
		dbSqlSessionFactory = new DbSqlSessionFactory();
		dbSqlSessionFactory.DatabaseType = databaseType;
		dbSqlSessionFactory.IdGenerator = idGenerator;
		dbSqlSessionFactory.SqlSessionFactory = sqlSessionFactory;
		dbSqlSessionFactory.DbIdentityUsed = isDbIdentityUsed;
		dbSqlSessionFactory.DbHistoryUsed = isDbHistoryUsed;
		dbSqlSessionFactory.CmmnEnabled = cmmnEnabled;
		dbSqlSessionFactory.DmnEnabled = dmnEnabled;
		dbSqlSessionFactory.DatabaseTablePrefix = databaseTablePrefix;

		//hack for the case when schema is defined via databaseTablePrefix parameter and not via databaseSchema parameter
		if (!string.ReferenceEquals(databaseTablePrefix, null) && string.ReferenceEquals(databaseSchema, null) && databaseTablePrefix.Contains("."))
		{
		  databaseSchema = databaseTablePrefix.Split("\\.", true)[0];
		}
		dbSqlSessionFactory.DatabaseSchema = databaseSchema;
		addSessionFactory(dbSqlSessionFactory);
		addSessionFactory(new DbSqlPersistenceProviderFactory());
	  }

	  protected internal virtual void initMigration()
	  {
		initMigrationInstructionValidators();
		initMigrationActivityMatcher();
		initMigrationInstructionGenerator();
		initMigratingActivityInstanceValidators();
		initMigratingTransitionInstanceValidators();
		initMigratingCompensationInstanceValidators();
	  }

	  protected internal virtual void initMigrationActivityMatcher()
	  {
		if (migrationActivityMatcher == null)
		{
		  migrationActivityMatcher = new DefaultMigrationActivityMatcher();
		}
	  }

	  protected internal virtual void initMigrationInstructionGenerator()
	  {
		if (migrationInstructionGenerator == null)
		{
		  migrationInstructionGenerator = new DefaultMigrationInstructionGenerator(migrationActivityMatcher);
		}

		IList<MigrationActivityValidator> migrationActivityValidators = new List<MigrationActivityValidator>();
		if (customPreMigrationActivityValidators != null)
		{
		  ((IList<MigrationActivityValidator>)migrationActivityValidators).AddRange(customPreMigrationActivityValidators);
		}
		((IList<MigrationActivityValidator>)migrationActivityValidators).AddRange(DefaultMigrationActivityValidators);
		if (customPostMigrationActivityValidators != null)
		{
		  ((IList<MigrationActivityValidator>)migrationActivityValidators).AddRange(customPostMigrationActivityValidators);
		}
		migrationInstructionGenerator = migrationInstructionGenerator.migrationActivityValidators(migrationActivityValidators).migrationInstructionValidators(migrationInstructionValidators);
	  }

	  protected internal virtual void initMigrationInstructionValidators()
	  {
		if (migrationInstructionValidators == null)
		{
		  migrationInstructionValidators = new List<MigrationInstructionValidator>();
		  if (customPreMigrationInstructionValidators != null)
		  {
			((IList<MigrationInstructionValidator>)migrationInstructionValidators).AddRange(customPreMigrationInstructionValidators);
		  }
		  ((IList<MigrationInstructionValidator>)migrationInstructionValidators).AddRange(DefaultMigrationInstructionValidators);
		  if (customPostMigrationInstructionValidators != null)
		  {
			((IList<MigrationInstructionValidator>)migrationInstructionValidators).AddRange(customPostMigrationInstructionValidators);
		  }
		}
	  }

	  protected internal virtual void initMigratingActivityInstanceValidators()
	  {
		if (migratingActivityInstanceValidators == null)
		{
		  migratingActivityInstanceValidators = new List<MigratingActivityInstanceValidator>();
		  if (customPreMigratingActivityInstanceValidators != null)
		  {
			((IList<MigratingActivityInstanceValidator>)migratingActivityInstanceValidators).AddRange(customPreMigratingActivityInstanceValidators);
		  }
		  ((IList<MigratingActivityInstanceValidator>)migratingActivityInstanceValidators).AddRange(DefaultMigratingActivityInstanceValidators);
		  if (customPostMigratingActivityInstanceValidators != null)
		  {
			((IList<MigratingActivityInstanceValidator>)migratingActivityInstanceValidators).AddRange(customPostMigratingActivityInstanceValidators);
		  }

		}
	  }

	  protected internal virtual void initMigratingTransitionInstanceValidators()
	  {
		if (migratingTransitionInstanceValidators == null)
		{
		  migratingTransitionInstanceValidators = new List<MigratingTransitionInstanceValidator>();
		  ((IList<MigratingTransitionInstanceValidator>)migratingTransitionInstanceValidators).AddRange(DefaultMigratingTransitionInstanceValidators);
		}
	  }

	  protected internal virtual void initMigratingCompensationInstanceValidators()
	  {
		if (migratingCompensationInstanceValidators == null)
		{
		  migratingCompensationInstanceValidators = new List<MigratingCompensationInstanceValidator>();

		  migratingCompensationInstanceValidators.Add(new NoUnmappedLeafInstanceValidator());
		  migratingCompensationInstanceValidators.Add(new NoUnmappedCompensationStartEventValidator());
		}
	  }


	  /// <summary>
	  /// When providing a schema and a prefix  the prefix has to be the schema ending with a dot.
	  /// </summary>
	  protected internal virtual void ensurePrefixAndSchemaFitToegether(string prefix, string schema)
	  {
		if (string.ReferenceEquals(schema, null))
		{
		  return;
		}
		else if (string.ReferenceEquals(prefix, null) || (!string.ReferenceEquals(prefix, null) && !prefix.StartsWith(schema + ".", StringComparison.Ordinal)))
		{
		  throw new ProcessEngineException("When setting a schema the prefix has to be schema + '.'. Received schema: " + schema + " prefix: " + prefix);
		}
	  }

	  protected internal virtual void addSessionFactory(SessionFactory sessionFactory)
	  {
		sessionFactories[sessionFactory.SessionType] = sessionFactory;
	  }

	  // deployers ////////////////////////////////////////////////////////////////

	  protected internal virtual void initDeployers()
	  {
		if (this.deployers == null)
		{
		  this.deployers = new List<Deployer>();
		  if (customPreDeployers != null)
		  {
			((IList<Deployer>)this.deployers).AddRange(customPreDeployers);
		  }
		  ((IList<Deployer>)this.deployers).AddRange(DefaultDeployers);
		  if (customPostDeployers != null)
		  {
			((IList<Deployer>)this.deployers).AddRange(customPostDeployers);
		  }
		}
		if (deploymentCache == null)
		{
		  IList<Deployer> deployers = new List<Deployer>();
		  if (customPreDeployers != null)
		  {
			((IList<Deployer>)deployers).AddRange(customPreDeployers);
		  }
		  ((IList<Deployer>)deployers).AddRange(DefaultDeployers);
		  if (customPostDeployers != null)
		  {
			((IList<Deployer>)deployers).AddRange(customPostDeployers);
		  }

		  initCacheFactory();
		  deploymentCache = new DeploymentCache(cacheFactory, cacheCapacity);
		  deploymentCache.Deployers = deployers;
		}
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Collection<? extends org.camunda.bpm.engine.impl.persistence.deploy.Deployer> getDefaultDeployers()
	  protected internal virtual ICollection<Deployer> DefaultDeployers
	  {
		  get
		  {
			IList<Deployer> defaultDeployers = new List<Deployer>();
    
			BpmnDeployer bpmnDeployer = BpmnDeployer;
			defaultDeployers.Add(bpmnDeployer);
    
			if (CmmnEnabled)
			{
			  CmmnDeployer cmmnDeployer = CmmnDeployer;
			  defaultDeployers.Add(cmmnDeployer);
			}
    
			if (DmnEnabled)
			{
			  DecisionRequirementsDefinitionDeployer decisionRequirementsDefinitionDeployer = DecisionRequirementsDefinitionDeployer;
			  DecisionDefinitionDeployer decisionDefinitionDeployer = DecisionDefinitionDeployer;
			  // the DecisionRequirementsDefinition cacheDeployer must be before the DecisionDefinitionDeployer
			  defaultDeployers.Add(decisionRequirementsDefinitionDeployer);
			  defaultDeployers.Add(decisionDefinitionDeployer);
			}
    
			return defaultDeployers;
		  }
	  }

	  protected internal virtual BpmnDeployer BpmnDeployer
	  {
		  get
		  {
			BpmnDeployer bpmnDeployer = new BpmnDeployer();
			bpmnDeployer.ExpressionManager = expressionManager;
			bpmnDeployer.IdGenerator = idGenerator;
    
			if (bpmnParseFactory == null)
			{
			  bpmnParseFactory = new DefaultBpmnParseFactory();
			}
    
			BpmnParser bpmnParser = new BpmnParser(expressionManager, bpmnParseFactory);
    
			if (preParseListeners != null)
			{
			  ((IList<BpmnParseListener>)bpmnParser.ParseListeners).AddRange(preParseListeners);
			}
			((IList<BpmnParseListener>)bpmnParser.ParseListeners).AddRange(DefaultBPMNParseListeners);
			if (postParseListeners != null)
			{
			  ((IList<BpmnParseListener>)bpmnParser.ParseListeners).AddRange(postParseListeners);
			}
    
			bpmnDeployer.BpmnParser = bpmnParser;
    
			return bpmnDeployer;
		  }
	  }

	  protected internal virtual IList<BpmnParseListener> DefaultBPMNParseListeners
	  {
		  get
		  {
			IList<BpmnParseListener> defaultListeners = new List<BpmnParseListener>();
			if (!org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_NONE.Equals(historyLevel))
			{
			  defaultListeners.Add(new HistoryParseListener(historyLevel, historyEventProducer));
			}
			if (isMetricsEnabled)
			{
			  defaultListeners.Add(new MetricsBpmnParseListener());
			}
			return defaultListeners;
		  }
	  }

	  protected internal virtual CmmnDeployer CmmnDeployer
	  {
		  get
		  {
			CmmnDeployer cmmnDeployer = new CmmnDeployer();
    
			cmmnDeployer.IdGenerator = idGenerator;
    
			if (cmmnTransformFactory == null)
			{
			  cmmnTransformFactory = new DefaultCmmnTransformFactory();
			}
    
			if (cmmnElementHandlerRegistry == null)
			{
			  cmmnElementHandlerRegistry = new DefaultCmmnElementHandlerRegistry();
			}
    
			CmmnTransformer cmmnTransformer = new CmmnTransformer(expressionManager, cmmnElementHandlerRegistry, cmmnTransformFactory);
    
			if (customPreCmmnTransformListeners != null)
			{
			  ((IList<CmmnTransformListener>)cmmnTransformer.TransformListeners).AddRange(customPreCmmnTransformListeners);
			}
			((IList<CmmnTransformListener>)cmmnTransformer.TransformListeners).AddRange(DefaultCmmnTransformListeners);
			if (customPostCmmnTransformListeners != null)
			{
			  ((IList<CmmnTransformListener>)cmmnTransformer.TransformListeners).AddRange(customPostCmmnTransformListeners);
			}
    
			cmmnDeployer.Transformer = cmmnTransformer;
    
			return cmmnDeployer;
		  }
	  }

	  protected internal virtual IList<CmmnTransformListener> DefaultCmmnTransformListeners
	  {
		  get
		  {
			IList<CmmnTransformListener> defaultListener = new List<CmmnTransformListener>();
			if (!org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_NONE.Equals(historyLevel))
			{
			  defaultListener.Add(new CmmnHistoryTransformListener(historyLevel, cmmnHistoryEventProducer));
			}
			if (isMetricsEnabled)
			{
			  defaultListener.Add(new MetricsCmmnTransformListener());
			}
			return defaultListener;
		  }
	  }

	  protected internal virtual DecisionDefinitionDeployer DecisionDefinitionDeployer
	  {
		  get
		  {
			DecisionDefinitionDeployer decisionDefinitionDeployer = new DecisionDefinitionDeployer();
			decisionDefinitionDeployer.IdGenerator = idGenerator;
			decisionDefinitionDeployer.Transformer = dmnEngineConfiguration.Transformer;
			return decisionDefinitionDeployer;
		  }
	  }

	  protected internal virtual DecisionRequirementsDefinitionDeployer DecisionRequirementsDefinitionDeployer
	  {
		  get
		  {
			DecisionRequirementsDefinitionDeployer drdDeployer = new DecisionRequirementsDefinitionDeployer();
			drdDeployer.IdGenerator = idGenerator;
			drdDeployer.Transformer = dmnEngineConfiguration.Transformer;
			return drdDeployer;
		  }
	  }

	  public virtual DmnEngine DmnEngine
	  {
		  get
		  {
			return dmnEngine;
		  }
		  set
		  {
			this.dmnEngine = value;
		  }
	  }


	  public virtual DefaultDmnEngineConfiguration DmnEngineConfiguration
	  {
		  get
		  {
			return dmnEngineConfiguration;
		  }
		  set
		  {
			this.dmnEngineConfiguration = value;
		  }
	  }


	  // job executor /////////////////////////////////////////////////////////////

	  protected internal virtual void initJobExecutor()
	  {
		if (jobExecutor == null)
		{
		  jobExecutor = new DefaultJobExecutor();
		}

		jobHandlers = new Dictionary<string, JobHandler>();
		TimerExecuteNestedActivityJobHandler timerExecuteNestedActivityJobHandler = new TimerExecuteNestedActivityJobHandler();
		jobHandlers[timerExecuteNestedActivityJobHandler.Type] = timerExecuteNestedActivityJobHandler;

		TimerCatchIntermediateEventJobHandler timerCatchIntermediateEvent = new TimerCatchIntermediateEventJobHandler();
		jobHandlers[timerCatchIntermediateEvent.Type] = timerCatchIntermediateEvent;

		TimerStartEventJobHandler timerStartEvent = new TimerStartEventJobHandler();
		jobHandlers[timerStartEvent.Type] = timerStartEvent;

		TimerStartEventSubprocessJobHandler timerStartEventSubprocess = new TimerStartEventSubprocessJobHandler();
		jobHandlers[timerStartEventSubprocess.Type] = timerStartEventSubprocess;

		AsyncContinuationJobHandler asyncContinuationJobHandler = new AsyncContinuationJobHandler();
		jobHandlers[asyncContinuationJobHandler.Type] = asyncContinuationJobHandler;

		ProcessEventJobHandler processEventJobHandler = new ProcessEventJobHandler();
		jobHandlers[processEventJobHandler.Type] = processEventJobHandler;

		TimerSuspendProcessDefinitionHandler suspendProcessDefinitionHandler = new TimerSuspendProcessDefinitionHandler();
		jobHandlers[suspendProcessDefinitionHandler.Type] = suspendProcessDefinitionHandler;

		TimerActivateProcessDefinitionHandler activateProcessDefinitionHandler = new TimerActivateProcessDefinitionHandler();
		jobHandlers[activateProcessDefinitionHandler.Type] = activateProcessDefinitionHandler;

		TimerSuspendJobDefinitionHandler suspendJobDefinitionHandler = new TimerSuspendJobDefinitionHandler();
		jobHandlers[suspendJobDefinitionHandler.Type] = suspendJobDefinitionHandler;

		TimerActivateJobDefinitionHandler activateJobDefinitionHandler = new TimerActivateJobDefinitionHandler();
		jobHandlers[activateJobDefinitionHandler.Type] = activateJobDefinitionHandler;

		BatchSeedJobHandler batchSeedJobHandler = new BatchSeedJobHandler();
		jobHandlers[batchSeedJobHandler.Type] = batchSeedJobHandler;

		BatchMonitorJobHandler batchMonitorJobHandler = new BatchMonitorJobHandler();
		jobHandlers[batchMonitorJobHandler.Type] = batchMonitorJobHandler;

		HistoryCleanupJobHandler historyCleanupJobHandler = new HistoryCleanupJobHandler();
		jobHandlers[historyCleanupJobHandler.Type] = historyCleanupJobHandler;

		foreach (JobHandler batchHandler in batchHandlers.Values)
		{
		  jobHandlers[batchHandler.Type] = batchHandler;
		}

		// if we have custom job handlers, register them
		if (CustomJobHandlers != null)
		{
		  foreach (JobHandler customJobHandler in CustomJobHandlers)
		  {
			jobHandlers[customJobHandler.Type] = customJobHandler;
		  }
		}

		jobExecutor.AutoActivate = jobExecutorActivate;

		if (jobExecutor.RejectedJobsHandler == null)
		{
		  if (customRejectedJobsHandler != null)
		  {
			jobExecutor.RejectedJobsHandler = customRejectedJobsHandler;
		  }
		  else
		  {
			jobExecutor.RejectedJobsHandler = new NotifyAcquisitionRejectedJobsHandler();
		  }
		}

	  }

	  protected internal virtual void initJobProvider()
	  {
		if (producePrioritizedJobs && jobPriorityProvider == null)
		{
		  jobPriorityProvider = new DefaultJobPriorityProvider();
		}
	  }

	  //external task /////////////////////////////////////////////////////////////

	  protected internal virtual void initExternalTaskPriorityProvider()
	  {
		if (producePrioritizedExternalTasks && externalTaskPriorityProvider == null)
		{
		  externalTaskPriorityProvider = new DefaultExternalTaskPriorityProvider();
		}
	  }

	  // history //////////////////////////////////////////////////////////////////

	  public virtual void initHistoryLevel()
	  {
		if (historyLevel != null)
		{
		  History = historyLevel.Name;
		}

		if (historyLevels == null)
		{
		  historyLevels = new List<HistoryLevel>();
		  historyLevels.Add(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_NONE);
		  historyLevels.Add(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_ACTIVITY);
		  historyLevels.Add(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_AUDIT);
		  historyLevels.Add(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL);
		}

		if (customHistoryLevels != null)
		{
		  ((IList<HistoryLevel>)historyLevels).AddRange(customHistoryLevels);
		}

		if (HISTORY_VARIABLE.Equals(history, StringComparison.OrdinalIgnoreCase))
		{
		  historyLevel = org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_ACTIVITY;
		  LOG.usingDeprecatedHistoryLevelVariable();
		}
		else
		{
		  foreach (HistoryLevel historyLevel in historyLevels)
		  {
			if (historyLevel.Name.Equals(history, StringComparison.OrdinalIgnoreCase))
			{
			  this.historyLevel = historyLevel;
			}
		  }
		}

		// do allow null for history level in case of "auto"
		if (historyLevel == null && !ProcessEngineConfiguration.HISTORY_AUTO.Equals(history, StringComparison.OrdinalIgnoreCase))
		{
		  throw new ProcessEngineException("invalid history level: " + history);
		}
	  }

	  // id generator /////////////////////////////////////////////////////////////

	  protected internal virtual void initIdGenerator()
	  {
		if (idGenerator == null)
		{
		  CommandExecutor idGeneratorCommandExecutor = null;
		  if (idGeneratorDataSource != null)
		  {
			ProcessEngineConfigurationImpl processEngineConfiguration = new StandaloneProcessEngineConfiguration();
			processEngineConfiguration.DataSource = idGeneratorDataSource;
			processEngineConfiguration.DatabaseSchemaUpdate = DB_SCHEMA_UPDATE_FALSE;
			processEngineConfiguration.init();
			idGeneratorCommandExecutor = processEngineConfiguration.CommandExecutorTxRequiresNew;
		  }
		  else if (!string.ReferenceEquals(idGeneratorDataSourceJndiName, null))
		  {
			ProcessEngineConfigurationImpl processEngineConfiguration = new StandaloneProcessEngineConfiguration();
			processEngineConfiguration.DataSourceJndiName = idGeneratorDataSourceJndiName;
			processEngineConfiguration.DatabaseSchemaUpdate = DB_SCHEMA_UPDATE_FALSE;
			processEngineConfiguration.init();
			idGeneratorCommandExecutor = processEngineConfiguration.CommandExecutorTxRequiresNew;
		  }
		  else
		  {
			idGeneratorCommandExecutor = commandExecutorTxRequiresNew;
		  }

		  DbIdGenerator dbIdGenerator = new DbIdGenerator();
		  dbIdGenerator.IdBlockSize = idBlockSize;
		  dbIdGenerator.CommandExecutor = idGeneratorCommandExecutor;
		  idGenerator = dbIdGenerator;
		}
	  }

	  // OTHER ////////////////////////////////////////////////////////////////////

	  protected internal virtual void initCommandContextFactory()
	  {
		if (commandContextFactory == null)
		{
		  commandContextFactory = new CommandContextFactory();
		  commandContextFactory.ProcessEngineConfiguration = this;
		}
	  }

	  protected internal virtual void initTransactionContextFactory()
	  {
		if (transactionContextFactory == null)
		{
		  transactionContextFactory = new StandaloneTransactionContextFactory();
		}
	  }

	  protected internal virtual void initValueTypeResolver()
	  {
		if (valueTypeResolver == null)
		{
		  valueTypeResolver = new ValueTypeResolverImpl();
		}
	  }

	  protected internal virtual void initDefaultCharset()
	  {
		if (defaultCharset == null)
		{
		  if (string.ReferenceEquals(defaultCharsetName, null))
		  {
			defaultCharsetName = "UTF-8";
		  }
		  defaultCharset = Charset.forName(defaultCharsetName);
		}
	  }

	  protected internal virtual void initMetrics()
	  {
		if (isMetricsEnabled)
		{

		  if (metricsReporterIdProvider == null)
		  {
			metricsReporterIdProvider = new SimpleIpBasedProvider();
		  }

		  if (metricsRegistry == null)
		  {
			metricsRegistry = new MetricsRegistry();
		  }

		  initDefaultMetrics(metricsRegistry);

		  if (dbMetricsReporter == null)
		  {
			dbMetricsReporter = new DbMetricsReporter(metricsRegistry, commandExecutorTxRequired);
		  }
		}
	  }

	  protected internal virtual void initDefaultMetrics(MetricsRegistry metricsRegistry)
	  {
		metricsRegistry.createMeter(Metrics.ACTIVTY_INSTANCE_START);
		metricsRegistry.createMeter(Metrics.ACTIVTY_INSTANCE_END);

		metricsRegistry.createMeter(Metrics.JOB_ACQUISITION_ATTEMPT);
		metricsRegistry.createMeter(Metrics.JOB_ACQUIRED_SUCCESS);
		metricsRegistry.createMeter(Metrics.JOB_ACQUIRED_FAILURE);
		metricsRegistry.createMeter(Metrics.JOB_SUCCESSFUL);
		metricsRegistry.createMeter(Metrics.JOB_FAILED);
		metricsRegistry.createMeter(Metrics.JOB_LOCKED_EXCLUSIVE);
		metricsRegistry.createMeter(Metrics.JOB_EXECUTION_REJECTED);

		metricsRegistry.createMeter(Metrics.EXECUTED_DECISION_ELEMENTS);
	  }

	  protected internal virtual void initSerialization()
	  {
		if (variableSerializers == null)
		{
		  variableSerializers = new DefaultVariableSerializers();

		  if (customPreVariableSerializers != null)
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?> customVariableType : customPreVariableSerializers)
			foreach (TypedValueSerializer<object> customVariableType in customPreVariableSerializers)
			{
			  variableSerializers.addSerializer(customVariableType);
			}
		  }

		  // register built-in serializers
		  variableSerializers.addSerializer(new NullValueSerializer());
		  variableSerializers.addSerializer(new StringValueSerializer());
		  variableSerializers.addSerializer(new BooleanValueSerializer());
		  variableSerializers.addSerializer(new ShortValueSerializer());
		  variableSerializers.addSerializer(new IntegerValueSerializer());
		  variableSerializers.addSerializer(new LongValueSerlializer());
		  variableSerializers.addSerializer(new DateValueSerializer());
		  variableSerializers.addSerializer(new DoubleValueSerializer());
		  variableSerializers.addSerializer(new ByteArrayValueSerializer());
		  variableSerializers.addSerializer(new JavaObjectSerializer());
		  variableSerializers.addSerializer(new FileValueSerializer());

		  if (customPostVariableSerializers != null)
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?> customVariableType : customPostVariableSerializers)
			foreach (TypedValueSerializer<object> customVariableType in customPostVariableSerializers)
			{
			  variableSerializers.addSerializer(customVariableType);
			}
		  }

		}
	  }

	  protected internal virtual void initFormEngines()
	  {
		if (formEngines == null)
		{
		  formEngines = new Dictionary<string, FormEngine>();
		  // html form engine = default form engine
		  FormEngine defaultFormEngine = new HtmlFormEngine();
		  formEngines[null] = defaultFormEngine; // default form engine is looked up with null
		  formEngines[defaultFormEngine.Name] = defaultFormEngine;
		  FormEngine juelFormEngine = new JuelFormEngine();
		  formEngines[juelFormEngine.Name] = juelFormEngine;

		}
		if (customFormEngines != null)
		{
		  foreach (FormEngine formEngine in customFormEngines)
		  {
			formEngines[formEngine.Name] = formEngine;
		  }
		}
	  }

	  protected internal virtual void initFormTypes()
	  {
		if (formTypes == null)
		{
		  formTypes = new FormTypes();
		  formTypes.addFormType(new StringFormType());
		  formTypes.addFormType(new LongFormType());
		  formTypes.addFormType(new DateFormType("dd/MM/yyyy"));
		  formTypes.addFormType(new BooleanFormType());
		}
		if (customFormTypes != null)
		{
		  foreach (AbstractFormFieldType customFormType in customFormTypes)
		  {
			formTypes.addFormType(customFormType);
		  }
		}
	  }

	  protected internal virtual void initFormFieldValidators()
	  {
		if (formValidators == null)
		{
		  formValidators = new FormValidators();
		  formValidators.addValidator("min", typeof(MinValidator));
		  formValidators.addValidator("max", typeof(MaxValidator));
		  formValidators.addValidator("minlength", typeof(MinLengthValidator));
		  formValidators.addValidator("maxlength", typeof(MaxLengthValidator));
		  formValidators.addValidator("required", typeof(RequiredValidator));
		  formValidators.addValidator("readonly", typeof(ReadOnlyValidator));
		}
		if (customFormFieldValidators != null)
		{
		  foreach (KeyValuePair<string, Type> validator in customFormFieldValidators.SetOfKeyValuePairs())
		  {
			formValidators.addValidator(validator.Key, validator.Value);
		  }
		}

	  }

	  protected internal virtual void initScripting()
	  {
		if (resolverFactories == null)
		{
		  resolverFactories = new List<ResolverFactory>();
		  resolverFactories.Add(new MocksResolverFactory());
		  resolverFactories.Add(new VariableScopeResolverFactory());
		  resolverFactories.Add(new BeansResolverFactory());
		}
		if (scriptingEngines == null)
		{
		  scriptingEngines = new ScriptingEngines(new ScriptBindingsFactory(resolverFactories));
		  scriptingEngines.EnableScriptEngineCaching = enableScriptEngineCaching;
		}
		if (scriptFactory == null)
		{
		  scriptFactory = new ScriptFactory();
		}
		if (scriptEnvResolvers == null)
		{
		  scriptEnvResolvers = new List<ScriptEnvResolver>();
		}
		if (scriptingEnvironment == null)
		{
		  scriptingEnvironment = new ScriptingEnvironment(scriptFactory, scriptEnvResolvers, scriptingEngines);
		}
	  }

	  protected internal virtual void initDmnEngine()
	  {
		if (dmnEngine == null)
		{

		  if (dmnEngineConfiguration == null)
		  {
			dmnEngineConfiguration = (DefaultDmnEngineConfiguration) DmnEngineConfiguration.createDefaultDmnEngineConfiguration();
		  }

		  dmnEngineConfiguration = (new DmnEngineConfigurationBuilder(dmnEngineConfiguration)).historyLevel(historyLevel).dmnHistoryEventProducer(dmnHistoryEventProducer).scriptEngineResolver(scriptingEngines).expressionManager(expressionManager).build();

		  dmnEngine = dmnEngineConfiguration.buildEngine();

		}
		else if (dmnEngineConfiguration == null)
		{
		  dmnEngineConfiguration = (DefaultDmnEngineConfiguration) dmnEngine.Configuration;
		}
	  }

	  protected internal virtual void initExpressionManager()
	  {
		if (expressionManager == null)
		{
		  expressionManager = new ExpressionManager(beans);
		}

		// add function mapper for command context (eg currentUser(), currentUserGroups())
		expressionManager.addFunctionMapper(new CommandContextFunctionMapper());
		// add function mapper for date time (eg now(), dateTime())
		expressionManager.addFunctionMapper(new DateTimeFunctionMapper());
	  }

	  protected internal virtual void initBusinessCalendarManager()
	  {
		if (businessCalendarManager == null)
		{
		  MapBusinessCalendarManager mapBusinessCalendarManager = new MapBusinessCalendarManager();
		  mapBusinessCalendarManager.addBusinessCalendar(DurationBusinessCalendar.NAME, new DurationBusinessCalendar());
		  mapBusinessCalendarManager.addBusinessCalendar(DueDateBusinessCalendar.NAME, new DueDateBusinessCalendar());
		  mapBusinessCalendarManager.addBusinessCalendar(CycleBusinessCalendar.NAME, new CycleBusinessCalendar());

		  businessCalendarManager = mapBusinessCalendarManager;
		}
	  }

	  protected internal virtual void initDelegateInterceptor()
	  {
		if (delegateInterceptor == null)
		{
		  delegateInterceptor = new DefaultDelegateInterceptor();
		}
	  }

	  protected internal virtual void initEventHandlers()
	  {
		if (eventHandlers == null)
		{
		  eventHandlers = new Dictionary<string, EventHandler>();

		  SignalEventHandler signalEventHander = new SignalEventHandler();
		  eventHandlers[signalEventHander.EventHandlerType] = signalEventHander;

		  CompensationEventHandler compensationEventHandler = new CompensationEventHandler();
		  eventHandlers[compensationEventHandler.EventHandlerType] = compensationEventHandler;

		  EventHandler messageEventHandler = new EventHandlerImpl(EventType.MESSAGE);
		  eventHandlers[messageEventHandler.EventHandlerType] = messageEventHandler;

		  EventHandler conditionalEventHandler = new ConditionalEventHandler();
		  eventHandlers[conditionalEventHandler.EventHandlerType] = conditionalEventHandler;

		}
		if (customEventHandlers != null)
		{
		  foreach (EventHandler eventHandler in customEventHandlers)
		  {
			eventHandlers[eventHandler.EventHandlerType] = eventHandler;
		  }
		}
	  }

	  protected internal virtual void initCommandCheckers()
	  {
		if (commandCheckers == null)
		{
		  commandCheckers = new List<CommandChecker>();

		  // add the default command checkers
		  commandCheckers.Add(new TenantCommandChecker());
		  commandCheckers.Add(new AuthorizationCommandChecker());
		}
	  }

	  // JPA //////////////////////////////////////////////////////////////////////

	  protected internal virtual void initJpa()
	  {
		if (!string.ReferenceEquals(jpaPersistenceUnitName, null))
		{
		  jpaEntityManagerFactory = JpaHelper.createEntityManagerFactory(jpaPersistenceUnitName);
		}
		if (jpaEntityManagerFactory != null)
		{
		  sessionFactories[typeof(EntityManagerSession)] = new EntityManagerSessionFactory(jpaEntityManagerFactory, jpaHandleTransaction, jpaCloseEntityManager);
		  JPAVariableSerializer jpaType = (JPAVariableSerializer) variableSerializers.getSerializerByName(JPAVariableSerializer.NAME);
		  // Add JPA-type
		  if (jpaType == null)
		  {
			// We try adding the variable right after byte serializer, if available
			int serializableIndex = variableSerializers.getSerializerIndexByName(ValueType.BYTES.Name);
			if (serializableIndex > -1)
			{
			  variableSerializers.addSerializer(new JPAVariableSerializer(), serializableIndex);
			}
			else
			{
			  variableSerializers.addSerializer(new JPAVariableSerializer());
			}
		  }
		}
	  }

	  protected internal virtual void initBeans()
	  {
		if (beans == null)
		{
		  beans = new Dictionary<object, object>();
		}
	  }

	  protected internal virtual void initArtifactFactory()
	  {
		if (artifactFactory == null)
		{
		  artifactFactory = new DefaultArtifactFactory();
		}
	  }

	  protected internal virtual void initProcessApplicationManager()
	  {
		if (processApplicationManager == null)
		{
		  processApplicationManager = new ProcessApplicationManager();
		}
	  }

	  // correlation handler //////////////////////////////////////////////////////
	  protected internal virtual void initCorrelationHandler()
	  {
		if (correlationHandler == null)
		{
		  correlationHandler = new DefaultCorrelationHandler();
		}

	  }

	  // condition handler //////////////////////////////////////////////////////
	  protected internal virtual void initConditionHandler()
	  {
		if (conditionHandler == null)
		{
		  conditionHandler = new DefaultConditionHandler();
		}
	  }

	  // history handlers /////////////////////////////////////////////////////

	  protected internal virtual void initHistoryEventProducer()
	  {
		if (historyEventProducer == null)
		{
		  historyEventProducer = new CacheAwareHistoryEventProducer();
		}
	  }

	  protected internal virtual void initCmmnHistoryEventProducer()
	  {
		if (cmmnHistoryEventProducer == null)
		{
		  cmmnHistoryEventProducer = new CacheAwareCmmnHistoryEventProducer();
		}
	  }

	  protected internal virtual void initDmnHistoryEventProducer()
	  {
		if (dmnHistoryEventProducer == null)
		{
		  dmnHistoryEventProducer = new DefaultDmnHistoryEventProducer();
		}
	  }

	  protected internal virtual void initHistoryEventHandler()
	  {
		if (historyEventHandler == null)
		{
		  historyEventHandler = new DbHistoryEventHandler();
		}
	  }

	  // password digest //////////////////////////////////////////////////////////

	  protected internal virtual void initPasswordDigest()
	  {
		if (saltGenerator == null)
		{
		  saltGenerator = new Default16ByteSaltGenerator();
		}
		if (passwordEncryptor == null)
		{
		  passwordEncryptor = new Sha512HashDigest();
		}
		if (customPasswordChecker == null)
		{
		  customPasswordChecker = Collections.emptyList();
		}
		if (passwordManager == null)
		{
		  passwordManager = new PasswordManager(passwordEncryptor, customPasswordChecker);
		}
	  }

	  public virtual void initPasswordPolicy()
	  {
		if (passwordPolicy == null && enablePasswordPolicy)
		{
		  passwordPolicy = new DefaultPasswordPolicyImpl();
		}
	  }

	  protected internal virtual void initDeploymentRegistration()
	  {
		if (registeredDeployments == null)
		{
		  registeredDeployments = new CopyOnWriteArraySet<string>();
		}
	  }

	  // cache factory //////////////////////////////////////////////////////////

	  protected internal virtual void initCacheFactory()
	  {
		if (cacheFactory == null)
		{
		  cacheFactory = new DefaultCacheFactory();
		}
	  }

	  // resource authorization provider //////////////////////////////////////////

	  protected internal virtual void initResourceAuthorizationProvider()
	  {
		if (resourceAuthorizationProvider == null)
		{
		  resourceAuthorizationProvider = new DefaultAuthorizationProvider();
		}
	  }

	  protected internal virtual void initPermissionProvider()
	  {
		if (permissionProvider == null)
		{
		  permissionProvider = new DefaultPermissionProvider();
		}
	  }

	  protected internal virtual void initDefaultUserPermissionForTask()
	  {
		if (defaultUserPermissionForTask == null)
		{
		  if (Permissions.UPDATE.Name.Equals(defaultUserPermissionNameForTask))
		  {
			defaultUserPermissionForTask = Permissions.UPDATE;
		  }
		  else if (Permissions.TASK_WORK.Name.Equals(defaultUserPermissionNameForTask))
		  {
			defaultUserPermissionForTask = Permissions.TASK_WORK;
		  }
		  else
		  {
			throw LOG.invalidConfigDefaultUserPermissionNameForTask(defaultUserPermissionNameForTask, new string[]{Permissions.UPDATE.Name, Permissions.TASK_WORK.Name});
		  }
		}
	  }

	  protected internal virtual void initAdminUser()
	  {
		if (adminUsers == null)
		{
		  adminUsers = new List<>();
		}
	  }

	  protected internal virtual void initAdminGroups()
	  {
		if (adminGroups == null)
		{
		  adminGroups = new List<string>();
		}
		if (adminGroups.Count == 0 || !(adminGroups.Contains(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN)))
		{
		  adminGroups.Add(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN);
		}
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public override string ProcessEngineName
	  {
		  get
		  {
			return processEngineName;
		  }
	  }

	  public virtual HistoryLevel HistoryLevel
	  {
		  get
		  {
			return historyLevel;
		  }
		  set
		  {
			this.historyLevel = value;
		  }
	  }


	  public virtual HistoryLevel DefaultHistoryLevel
	  {
		  get
		  {
			if (historyLevels != null)
			{
			  foreach (HistoryLevel historyLevel in historyLevels)
			  {
				if (!string.ReferenceEquals(HISTORY_DEFAULT, null) && HISTORY_DEFAULT.Equals(historyLevel.Name, StringComparison.OrdinalIgnoreCase))
				{
				  return historyLevel;
				}
			  }
			}
    
			return null;
		  }
	  }

	  public override ProcessEngineConfigurationImpl setProcessEngineName(string processEngineName)
	  {
		this.processEngineName = processEngineName;
		return this;
	  }

	  public virtual IList<CommandInterceptor> CustomPreCommandInterceptorsTxRequired
	  {
		  get
		  {
			return customPreCommandInterceptorsTxRequired;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomPreCommandInterceptorsTxRequired(IList<CommandInterceptor> customPreCommandInterceptorsTxRequired)
	  {
		this.customPreCommandInterceptorsTxRequired = customPreCommandInterceptorsTxRequired;
		return this;
	  }

	  public virtual IList<CommandInterceptor> CustomPostCommandInterceptorsTxRequired
	  {
		  get
		  {
			return customPostCommandInterceptorsTxRequired;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomPostCommandInterceptorsTxRequired(IList<CommandInterceptor> customPostCommandInterceptorsTxRequired)
	  {
		this.customPostCommandInterceptorsTxRequired = customPostCommandInterceptorsTxRequired;
		return this;
	  }

	  public virtual IList<CommandInterceptor> CommandInterceptorsTxRequired
	  {
		  get
		  {
			return commandInterceptorsTxRequired;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCommandInterceptorsTxRequired(IList<CommandInterceptor> commandInterceptorsTxRequired)
	  {
		this.commandInterceptorsTxRequired = commandInterceptorsTxRequired;
		return this;
	  }

	  public virtual CommandExecutor CommandExecutorTxRequired
	  {
		  get
		  {
			return commandExecutorTxRequired;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCommandExecutorTxRequired(CommandExecutor commandExecutorTxRequired)
	  {
		this.commandExecutorTxRequired = commandExecutorTxRequired;
		return this;
	  }

	  public virtual IList<CommandInterceptor> CustomPreCommandInterceptorsTxRequiresNew
	  {
		  get
		  {
			return customPreCommandInterceptorsTxRequiresNew;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomPreCommandInterceptorsTxRequiresNew(IList<CommandInterceptor> customPreCommandInterceptorsTxRequiresNew)
	  {
		this.customPreCommandInterceptorsTxRequiresNew = customPreCommandInterceptorsTxRequiresNew;
		return this;
	  }

	  public virtual IList<CommandInterceptor> CustomPostCommandInterceptorsTxRequiresNew
	  {
		  get
		  {
			return customPostCommandInterceptorsTxRequiresNew;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomPostCommandInterceptorsTxRequiresNew(IList<CommandInterceptor> customPostCommandInterceptorsTxRequiresNew)
	  {
		this.customPostCommandInterceptorsTxRequiresNew = customPostCommandInterceptorsTxRequiresNew;
		return this;
	  }

	  public virtual IList<CommandInterceptor> CommandInterceptorsTxRequiresNew
	  {
		  get
		  {
			return commandInterceptorsTxRequiresNew;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCommandInterceptorsTxRequiresNew(IList<CommandInterceptor> commandInterceptorsTxRequiresNew)
	  {
		this.commandInterceptorsTxRequiresNew = commandInterceptorsTxRequiresNew;
		return this;
	  }

	  public virtual CommandExecutor CommandExecutorTxRequiresNew
	  {
		  get
		  {
			return commandExecutorTxRequiresNew;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCommandExecutorTxRequiresNew(CommandExecutor commandExecutorTxRequiresNew)
	  {
		this.commandExecutorTxRequiresNew = commandExecutorTxRequiresNew;
		return this;
	  }

	  public virtual RepositoryService RepositoryService
	  {
		  get
		  {
			return repositoryService;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setRepositoryService(RepositoryService repositoryService)
	  {
		this.repositoryService = repositoryService;
		return this;
	  }

	  public virtual RuntimeService RuntimeService
	  {
		  get
		  {
			return runtimeService;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setRuntimeService(RuntimeService runtimeService)
	  {
		this.runtimeService = runtimeService;
		return this;
	  }

	  public virtual HistoryService HistoryService
	  {
		  get
		  {
			return historyService;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setHistoryService(HistoryService historyService)
	  {
		this.historyService = historyService;
		return this;
	  }

	  public virtual IdentityService IdentityService
	  {
		  get
		  {
			return identityService;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setIdentityService(IdentityService identityService)
	  {
		this.identityService = identityService;
		return this;
	  }

	  public virtual TaskService TaskService
	  {
		  get
		  {
			return taskService;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setTaskService(TaskService taskService)
	  {
		this.taskService = taskService;
		return this;
	  }

	  public virtual FormService FormService
	  {
		  get
		  {
			return formService;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setFormService(FormService formService)
	  {
		this.formService = formService;
		return this;
	  }

	  public virtual ManagementService ManagementService
	  {
		  get
		  {
			return managementService;
		  }
	  }

	  public virtual AuthorizationService AuthorizationService
	  {
		  get
		  {
			return authorizationService;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setManagementService(ManagementService managementService)
	  {
		this.managementService = managementService;
		return this;
	  }

	  public virtual CaseService CaseService
	  {
		  get
		  {
			return caseService;
		  }
		  set
		  {
			this.caseService = value;
		  }
	  }


	  public virtual FilterService FilterService
	  {
		  get
		  {
			return filterService;
		  }
		  set
		  {
			this.filterService = value;
		  }
	  }


	  public virtual ExternalTaskService ExternalTaskService
	  {
		  get
		  {
			return externalTaskService;
		  }
		  set
		  {
			this.externalTaskService = value;
		  }
	  }


	  public virtual DecisionService DecisionService
	  {
		  get
		  {
			return decisionService;
		  }
		  set
		  {
			this.decisionService = value;
		  }
	  }

	  public virtual OptimizeService OptimizeService
	  {
		  get
		  {
			return optimizeService;
		  }
	  }


	  public virtual IDictionary<Type, SessionFactory> SessionFactories
	  {
		  get
		  {
			return sessionFactories;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setSessionFactories(IDictionary<Type, SessionFactory> sessionFactories)
	  {
		this.sessionFactories = sessionFactories;
		return this;
	  }

	  public virtual IList<Deployer> Deployers
	  {
		  get
		  {
			return deployers;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setDeployers(IList<Deployer> deployers)
	  {
		this.deployers = deployers;
		return this;
	  }

	  public virtual JobExecutor JobExecutor
	  {
		  get
		  {
			return jobExecutor;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setJobExecutor(JobExecutor jobExecutor)
	  {
		this.jobExecutor = jobExecutor;
		return this;
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.PriorityProvider<org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration<?, ?>> getJobPriorityProvider()
	  public virtual PriorityProvider<JobDeclaration<object, ?>> JobPriorityProvider
	  {
		  get
		  {
			return jobPriorityProvider;
		  }
		  set
		  {
			this.jobPriorityProvider = value;
		  }
	  }


	  public virtual PriorityProvider<ExternalTaskActivityBehavior> ExternalTaskPriorityProvider
	  {
		  get
		  {
			return externalTaskPriorityProvider;
		  }
		  set
		  {
			this.externalTaskPriorityProvider = value;
		  }
	  }


	  public virtual IdGenerator IdGenerator
	  {
		  get
		  {
			return idGenerator;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setIdGenerator(IdGenerator idGenerator)
	  {
		this.idGenerator = idGenerator;
		return this;
	  }

	  public virtual string WsSyncFactoryClassName
	  {
		  get
		  {
			return wsSyncFactoryClassName;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setWsSyncFactoryClassName(string wsSyncFactoryClassName)
	  {
		this.wsSyncFactoryClassName = wsSyncFactoryClassName;
		return this;
	  }

	  public virtual IDictionary<string, FormEngine> FormEngines
	  {
		  get
		  {
			return formEngines;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setFormEngines(IDictionary<string, FormEngine> formEngines)
	  {
		this.formEngines = formEngines;
		return this;
	  }

	  public virtual FormTypes FormTypes
	  {
		  get
		  {
			return formTypes;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setFormTypes(FormTypes formTypes)
	  {
		this.formTypes = formTypes;
		return this;
	  }

	  public virtual ScriptingEngines ScriptingEngines
	  {
		  get
		  {
			return scriptingEngines;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setScriptingEngines(ScriptingEngines scriptingEngines)
	  {
		this.scriptingEngines = scriptingEngines;
		return this;
	  }

	  public virtual VariableSerializers VariableSerializers
	  {
		  get
		  {
			return variableSerializers;
		  }
	  }

	  public virtual VariableSerializerFactory FallbackSerializerFactory
	  {
		  get
		  {
			return fallbackSerializerFactory;
		  }
		  set
		  {
			this.fallbackSerializerFactory = value;
		  }
	  }


	  public virtual ProcessEngineConfigurationImpl setVariableTypes(VariableSerializers variableSerializers)
	  {
		this.variableSerializers = variableSerializers;
		return this;
	  }

	  public virtual ExpressionManager ExpressionManager
	  {
		  get
		  {
			return expressionManager;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setExpressionManager(ExpressionManager expressionManager)
	  {
		this.expressionManager = expressionManager;
		return this;
	  }

	  public virtual BusinessCalendarManager BusinessCalendarManager
	  {
		  get
		  {
			return businessCalendarManager;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setBusinessCalendarManager(BusinessCalendarManager businessCalendarManager)
	  {
		this.businessCalendarManager = businessCalendarManager;
		return this;
	  }

	  public virtual CommandContextFactory CommandContextFactory
	  {
		  get
		  {
			return commandContextFactory;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCommandContextFactory(CommandContextFactory commandContextFactory)
	  {
		this.commandContextFactory = commandContextFactory;
		return this;
	  }

	  public virtual TransactionContextFactory TransactionContextFactory
	  {
		  get
		  {
			return transactionContextFactory;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setTransactionContextFactory(TransactionContextFactory transactionContextFactory)
	  {
		this.transactionContextFactory = transactionContextFactory;
		return this;
	  }


	  public virtual IList<Deployer> CustomPreDeployers
	  {
		  get
		  {
			return customPreDeployers;
		  }
	  }


	  public virtual ProcessEngineConfigurationImpl setCustomPreDeployers(IList<Deployer> customPreDeployers)
	  {
		this.customPreDeployers = customPreDeployers;
		return this;
	  }


	  public virtual IList<Deployer> CustomPostDeployers
	  {
		  get
		  {
			return customPostDeployers;
		  }
	  }


	  public virtual ProcessEngineConfigurationImpl setCustomPostDeployers(IList<Deployer> customPostDeployers)
	  {
		this.customPostDeployers = customPostDeployers;
		return this;
	  }

	  public virtual CacheFactory CacheFactory
	  {
		  set
		  {
			this.cacheFactory = value;
		  }
	  }

	  public virtual int CacheCapacity
	  {
		  set
		  {
			this.cacheCapacity = value;
		  }
	  }

	  public virtual bool EnableFetchProcessDefinitionDescription
	  {
		  set
		  {
			this.enableFetchProcessDefinitionDescription = value;
		  }
		  get
		  {
			return this.enableFetchProcessDefinitionDescription;
		  }
	  }


	  public virtual Permission DefaultUserPermissionForTask
	  {
		  get
		  {
			return defaultUserPermissionForTask;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setDefaultUserPermissionForTask(Permission defaultUserPermissionForTask)
	  {
		this.defaultUserPermissionForTask = defaultUserPermissionForTask;
		return this;
	  }

	  public virtual IDictionary<string, JobHandler> JobHandlers
	  {
		  get
		  {
			return jobHandlers;
		  }
	  }


	  public virtual ProcessEngineConfigurationImpl setJobHandlers(IDictionary<string, JobHandler> jobHandlers)
	  {
		this.jobHandlers = jobHandlers;
		return this;
	  }


	  public virtual SqlSessionFactory SqlSessionFactory
	  {
		  get
		  {
			return sqlSessionFactory;
		  }
	  }


	  public virtual ProcessEngineConfigurationImpl setSqlSessionFactory(SqlSessionFactory sqlSessionFactory)
	  {
		this.sqlSessionFactory = sqlSessionFactory;
		return this;
	  }


	  public virtual DbSqlSessionFactory DbSqlSessionFactory
	  {
		  get
		  {
			return dbSqlSessionFactory;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setDbSqlSessionFactory(DbSqlSessionFactory dbSqlSessionFactory)
	  {
		this.dbSqlSessionFactory = dbSqlSessionFactory;
		return this;
	  }

	  public virtual TransactionFactory TransactionFactory
	  {
		  get
		  {
			return transactionFactory;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setTransactionFactory(TransactionFactory transactionFactory)
	  {
		this.transactionFactory = transactionFactory;
		return this;
	  }

	  public virtual IList<SessionFactory> CustomSessionFactories
	  {
		  get
		  {
			return customSessionFactories;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomSessionFactories(IList<SessionFactory> customSessionFactories)
	  {
		this.customSessionFactories = customSessionFactories;
		return this;
	  }

	  public virtual IList<JobHandler> CustomJobHandlers
	  {
		  get
		  {
			return customJobHandlers;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomJobHandlers(IList<JobHandler> customJobHandlers)
	  {
		this.customJobHandlers = customJobHandlers;
		return this;
	  }

	  public virtual IList<FormEngine> CustomFormEngines
	  {
		  get
		  {
			return customFormEngines;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomFormEngines(IList<FormEngine> customFormEngines)
	  {
		this.customFormEngines = customFormEngines;
		return this;
	  }

	  public virtual IList<AbstractFormFieldType> CustomFormTypes
	  {
		  get
		  {
			return customFormTypes;
		  }
	  }


	  public virtual ProcessEngineConfigurationImpl setCustomFormTypes(IList<AbstractFormFieldType> customFormTypes)
	  {
		this.customFormTypes = customFormTypes;
		return this;
	  }

	  public virtual IList<TypedValueSerializer> CustomPreVariableSerializers
	  {
		  get
		  {
			return customPreVariableSerializers;
		  }
	  }


	  public virtual ProcessEngineConfigurationImpl setCustomPreVariableSerializers(IList<TypedValueSerializer> customPreVariableTypes)
	  {
		this.customPreVariableSerializers = customPreVariableTypes;
		return this;
	  }


	  public virtual IList<TypedValueSerializer> CustomPostVariableSerializers
	  {
		  get
		  {
			return customPostVariableSerializers;
		  }
	  }


	  public virtual ProcessEngineConfigurationImpl setCustomPostVariableSerializers(IList<TypedValueSerializer> customPostVariableTypes)
	  {
		this.customPostVariableSerializers = customPostVariableTypes;
		return this;
	  }

	  public virtual IList<BpmnParseListener> CustomPreBPMNParseListeners
	  {
		  get
		  {
			return preParseListeners;
		  }
		  set
		  {
			this.preParseListeners = value;
		  }
	  }


	  public virtual IList<BpmnParseListener> CustomPostBPMNParseListeners
	  {
		  get
		  {
			return postParseListeners;
		  }
		  set
		  {
			this.postParseListeners = value;
		  }
	  }


	  /// @deprecated use <seealso cref="getCustomPreBPMNParseListeners"/> instead. 
	  [Obsolete("use <seealso cref=\"getCustomPreBPMNParseListeners\"/> instead.")]
	  public virtual IList<BpmnParseListener> PreParseListeners
	  {
		  get
		  {
			return preParseListeners;
		  }
		  set
		  {
			this.preParseListeners = value;
		  }
	  }


	  /// @deprecated use <seealso cref="getCustomPostBPMNParseListeners"/> instead. 
	  [Obsolete("use <seealso cref=\"getCustomPostBPMNParseListeners\"/> instead.")]
	  public virtual IList<BpmnParseListener> PostParseListeners
	  {
		  get
		  {
			return postParseListeners;
		  }
		  set
		  {
			this.postParseListeners = value;
		  }
	  }


	  public virtual IList<CmmnTransformListener> CustomPreCmmnTransformListeners
	  {
		  get
		  {
			return customPreCmmnTransformListeners;
		  }
		  set
		  {
			this.customPreCmmnTransformListeners = value;
		  }
	  }


	  public virtual IList<CmmnTransformListener> CustomPostCmmnTransformListeners
	  {
		  get
		  {
			return customPostCmmnTransformListeners;
		  }
		  set
		  {
			this.customPostCmmnTransformListeners = value;
		  }
	  }


	  public virtual IDictionary<object, object> Beans
	  {
		  get
		  {
			return beans;
		  }
		  set
		  {
			this.beans = value;
		  }
	  }


	  public override ProcessEngineConfigurationImpl setClassLoader(ClassLoader classLoader)
	  {
		base.ClassLoader = classLoader;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setDatabaseType(string databaseType)
	  {
		base.DatabaseType = databaseType;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setDataSource(DataSource dataSource)
	  {
		base.DataSource = dataSource;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setDatabaseSchemaUpdate(string databaseSchemaUpdate)
	  {
		base.DatabaseSchemaUpdate = databaseSchemaUpdate;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setHistory(string history)
	  {
		base.History = history;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setIdBlockSize(int idBlockSize)
	  {
		base.IdBlockSize = idBlockSize;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setJdbcDriver(string jdbcDriver)
	  {
		base.JdbcDriver = jdbcDriver;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setJdbcPassword(string jdbcPassword)
	  {
		base.JdbcPassword = jdbcPassword;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setJdbcUrl(string jdbcUrl)
	  {
		base.JdbcUrl = jdbcUrl;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setJdbcUsername(string jdbcUsername)
	  {
		base.JdbcUsername = jdbcUsername;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setJobExecutorActivate(bool jobExecutorActivate)
	  {
		base.JobExecutorActivate = jobExecutorActivate;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setMailServerDefaultFrom(string mailServerDefaultFrom)
	  {
		base.MailServerDefaultFrom = mailServerDefaultFrom;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setMailServerHost(string mailServerHost)
	  {
		base.MailServerHost = mailServerHost;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setMailServerPassword(string mailServerPassword)
	  {
		base.MailServerPassword = mailServerPassword;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setMailServerPort(int mailServerPort)
	  {
		base.MailServerPort = mailServerPort;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setMailServerUseTLS(bool useTLS)
	  {
		base.MailServerUseTLS = useTLS;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setMailServerUsername(string mailServerUsername)
	  {
		base.MailServerUsername = mailServerUsername;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setJdbcMaxActiveConnections(int jdbcMaxActiveConnections)
	  {
		base.JdbcMaxActiveConnections = jdbcMaxActiveConnections;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setJdbcMaxCheckoutTime(int jdbcMaxCheckoutTime)
	  {
		base.JdbcMaxCheckoutTime = jdbcMaxCheckoutTime;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setJdbcMaxIdleConnections(int jdbcMaxIdleConnections)
	  {
		base.JdbcMaxIdleConnections = jdbcMaxIdleConnections;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setJdbcMaxWaitTime(int jdbcMaxWaitTime)
	  {
		base.JdbcMaxWaitTime = jdbcMaxWaitTime;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setTransactionsExternallyManaged(bool transactionsExternallyManaged)
	  {
		base.TransactionsExternallyManaged = transactionsExternallyManaged;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setJpaEntityManagerFactory(object jpaEntityManagerFactory)
	  {
		this.jpaEntityManagerFactory = jpaEntityManagerFactory;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setJpaHandleTransaction(bool jpaHandleTransaction)
	  {
		this.jpaHandleTransaction = jpaHandleTransaction;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setJpaCloseEntityManager(bool jpaCloseEntityManager)
	  {
		this.jpaCloseEntityManager = jpaCloseEntityManager;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setJdbcPingEnabled(bool jdbcPingEnabled)
	  {
		this.jdbcPingEnabled = jdbcPingEnabled;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setJdbcPingQuery(string jdbcPingQuery)
	  {
		this.jdbcPingQuery = jdbcPingQuery;
		return this;
	  }

	  public override ProcessEngineConfigurationImpl setJdbcPingConnectionNotUsedFor(int jdbcPingNotUsedFor)
	  {
		this.jdbcPingConnectionNotUsedFor = jdbcPingNotUsedFor;
		return this;
	  }

	  public virtual bool DbIdentityUsed
	  {
		  get
		  {
			return isDbIdentityUsed;
		  }
		  set
		  {
			this.isDbIdentityUsed = value;
		  }
	  }



	  public virtual bool DbHistoryUsed
	  {
		  get
		  {
			return isDbHistoryUsed;
		  }
		  set
		  {
			this.isDbHistoryUsed = value;
		  }
	  }


	  public virtual IList<ResolverFactory> ResolverFactories
	  {
		  get
		  {
			return resolverFactories;
		  }
		  set
		  {
			this.resolverFactories = value;
		  }
	  }


	  public virtual DeploymentCache DeploymentCache
	  {
		  get
		  {
			return deploymentCache;
		  }
		  set
		  {
			this.deploymentCache = value;
		  }
	  }


	  public virtual ProcessEngineConfigurationImpl setDelegateInterceptor(DelegateInterceptor delegateInterceptor)
	  {
		this.delegateInterceptor = delegateInterceptor;
		return this;
	  }

	  public virtual DelegateInterceptor DelegateInterceptor
	  {
		  get
		  {
			return delegateInterceptor;
		  }
	  }

	  public virtual RejectedJobsHandler CustomRejectedJobsHandler
	  {
		  get
		  {
			return customRejectedJobsHandler;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomRejectedJobsHandler(RejectedJobsHandler customRejectedJobsHandler)
	  {
		this.customRejectedJobsHandler = customRejectedJobsHandler;
		return this;
	  }

	  public virtual EventHandler getEventHandler(string eventType)
	  {
		return eventHandlers[eventType];
	  }

	  public virtual IDictionary<string, EventHandler> EventHandlers
	  {
		  set
		  {
			this.eventHandlers = value;
		  }
		  get
		  {
			return eventHandlers;
		  }
	  }


	  public virtual IList<EventHandler> CustomEventHandlers
	  {
		  get
		  {
			return customEventHandlers;
		  }
		  set
		  {
			this.customEventHandlers = value;
		  }
	  }


	  public virtual FailedJobCommandFactory FailedJobCommandFactory
	  {
		  get
		  {
			return failedJobCommandFactory;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setFailedJobCommandFactory(FailedJobCommandFactory failedJobCommandFactory)
	  {
		this.failedJobCommandFactory = failedJobCommandFactory;
		return this;
	  }

	  /// <summary>
	  /// Allows configuring a database table prefix which is used for all runtime operations of the process engine.
	  /// For example, if you specify a prefix named 'PRE1.', activiti will query for executions in a table named
	  /// 'PRE1.ACT_RU_EXECUTION_'.
	  /// <para>
	  /// <p/>
	  /// <strong>NOTE: the prefix is not respected by automatic database schema management. If you use
	  /// <seealso cref="ProcessEngineConfiguration.DB_SCHEMA_UPDATE_CREATE_DROP"/>
	  /// or <seealso cref="ProcessEngineConfiguration.DB_SCHEMA_UPDATE_TRUE"/>, activiti will create the database tables
	  /// using the default names, regardless of the prefix configured here.</strong>
	  /// 
	  /// @since 5.9
	  /// </para>
	  /// </summary>
	  public virtual ProcessEngineConfiguration setDatabaseTablePrefix(string databaseTablePrefix)
	  {
		this.databaseTablePrefix = databaseTablePrefix;
		return this;
	  }

	  public virtual string DatabaseTablePrefix
	  {
		  get
		  {
			return databaseTablePrefix;
		  }
	  }

	  public virtual bool CreateDiagramOnDeploy
	  {
		  get
		  {
			return isCreateDiagramOnDeploy;
		  }
	  }

	  public virtual ProcessEngineConfiguration setCreateDiagramOnDeploy(bool createDiagramOnDeploy)
	  {
		this.isCreateDiagramOnDeploy = createDiagramOnDeploy;
		return this;
	  }

	  public virtual string DatabaseSchema
	  {
		  get
		  {
			return databaseSchema;
		  }
		  set
		  {
			this.databaseSchema = value;
		  }
	  }


	  public virtual DataSource IdGeneratorDataSource
	  {
		  get
		  {
			return idGeneratorDataSource;
		  }
		  set
		  {
			this.idGeneratorDataSource = value;
		  }
	  }


	  public virtual string IdGeneratorDataSourceJndiName
	  {
		  get
		  {
			return idGeneratorDataSourceJndiName;
		  }
		  set
		  {
			this.idGeneratorDataSourceJndiName = value;
		  }
	  }


	  public virtual ProcessApplicationManager ProcessApplicationManager
	  {
		  get
		  {
			return processApplicationManager;
		  }
		  set
		  {
			this.processApplicationManager = value;
		  }
	  }


	  public virtual CommandExecutor CommandExecutorSchemaOperations
	  {
		  get
		  {
			return commandExecutorSchemaOperations;
		  }
		  set
		  {
			this.commandExecutorSchemaOperations = value;
		  }
	  }


	  public virtual CorrelationHandler CorrelationHandler
	  {
		  get
		  {
			return correlationHandler;
		  }
		  set
		  {
			this.correlationHandler = value;
		  }
	  }


	  public virtual ConditionHandler ConditionHandler
	  {
		  get
		  {
			return conditionHandler;
		  }
		  set
		  {
			this.conditionHandler = value;
		  }
	  }


	  public virtual ProcessEngineConfigurationImpl setHistoryEventHandler(HistoryEventHandler historyEventHandler)
	  {
		this.historyEventHandler = historyEventHandler;
		return this;
	  }

	  public virtual HistoryEventHandler HistoryEventHandler
	  {
		  get
		  {
			return historyEventHandler;
		  }
	  }

	  public virtual IncidentHandler getIncidentHandler(string incidentType)
	  {
		return incidentHandlers[incidentType];
	  }

	  public virtual IDictionary<string, IncidentHandler> IncidentHandlers
	  {
		  get
		  {
			return incidentHandlers;
		  }
		  set
		  {
			this.incidentHandlers = value;
		  }
	  }


	  public virtual IList<IncidentHandler> CustomIncidentHandlers
	  {
		  get
		  {
			return customIncidentHandlers;
		  }
		  set
		  {
			this.customIncidentHandlers = value;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.Map<String, org.camunda.bpm.engine.impl.batch.BatchJobHandler<?>> getBatchHandlers()
	  public virtual IDictionary<string, BatchJobHandler<object>> BatchHandlers
	  {
		  get
		  {
			return batchHandlers;
		  }
		  set
		  {
			this.batchHandlers = value;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.List<org.camunda.bpm.engine.impl.batch.BatchJobHandler<?>> getCustomBatchJobHandlers()
	  public virtual IList<BatchJobHandler<object>> CustomBatchJobHandlers
	  {
		  get
		  {
			return customBatchJobHandlers;
		  }
		  set
		  {
			this.customBatchJobHandlers = value;
		  }
	  }


	  public virtual int BatchJobsPerSeed
	  {
		  get
		  {
			return batchJobsPerSeed;
		  }
		  set
		  {
			this.batchJobsPerSeed = value;
		  }
	  }


	  public virtual int InvocationsPerBatchJob
	  {
		  get
		  {
			return invocationsPerBatchJob;
		  }
		  set
		  {
			this.invocationsPerBatchJob = value;
		  }
	  }


	  public virtual int BatchPollTime
	  {
		  get
		  {
			return batchPollTime;
		  }
		  set
		  {
			this.batchPollTime = value;
		  }
	  }


	  public virtual long BatchJobPriority
	  {
		  get
		  {
			return batchJobPriority;
		  }
		  set
		  {
			this.batchJobPriority = value;
		  }
	  }


	  public virtual SessionFactory IdentityProviderSessionFactory
	  {
		  get
		  {
			return identityProviderSessionFactory;
		  }
		  set
		  {
			this.identityProviderSessionFactory = value;
		  }
	  }


	  public virtual SaltGenerator SaltGenerator
	  {
		  get
		  {
			return saltGenerator;
		  }
		  set
		  {
			this.saltGenerator = value;
		  }
	  }


	  public virtual PasswordEncryptor PasswordEncryptor
	  {
		  set
		  {
			this.passwordEncryptor = value;
		  }
		  get
		  {
			return passwordEncryptor;
		  }
	  }


	  public virtual IList<PasswordEncryptor> CustomPasswordChecker
	  {
		  get
		  {
			return customPasswordChecker;
		  }
		  set
		  {
			this.customPasswordChecker = value;
		  }
	  }


	  public virtual PasswordManager PasswordManager
	  {
		  get
		  {
			return passwordManager;
		  }
		  set
		  {
			this.passwordManager = value;
		  }
	  }


	  public virtual ISet<string> RegisteredDeployments
	  {
		  get
		  {
			return registeredDeployments;
		  }
		  set
		  {
			this.registeredDeployments = value;
		  }
	  }


	  public virtual ResourceAuthorizationProvider ResourceAuthorizationProvider
	  {
		  get
		  {
			return resourceAuthorizationProvider;
		  }
		  set
		  {
			this.resourceAuthorizationProvider = value;
		  }
	  }


	  public virtual PermissionProvider PermissionProvider
	  {
		  get
		  {
			return permissionProvider;
		  }
		  set
		  {
			this.permissionProvider = value;
		  }
	  }


	  public virtual IList<ProcessEnginePlugin> ProcessEnginePlugins
	  {
		  get
		  {
			return processEnginePlugins;
		  }
		  set
		  {
			this.processEnginePlugins = value;
		  }
	  }


	  public virtual ProcessEngineConfigurationImpl setHistoryEventProducer(HistoryEventProducer historyEventProducer)
	  {
		this.historyEventProducer = historyEventProducer;
		return this;
	  }

	  public virtual HistoryEventProducer HistoryEventProducer
	  {
		  get
		  {
			return historyEventProducer;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setCmmnHistoryEventProducer(CmmnHistoryEventProducer cmmnHistoryEventProducer)
	  {
		this.cmmnHistoryEventProducer = cmmnHistoryEventProducer;
		return this;
	  }

	  public virtual CmmnHistoryEventProducer CmmnHistoryEventProducer
	  {
		  get
		  {
			return cmmnHistoryEventProducer;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setDmnHistoryEventProducer(DmnHistoryEventProducer dmnHistoryEventProducer)
	  {
		this.dmnHistoryEventProducer = dmnHistoryEventProducer;
		return this;
	  }

	  public virtual DmnHistoryEventProducer DmnHistoryEventProducer
	  {
		  get
		  {
			return dmnHistoryEventProducer;
		  }
	  }

	  public virtual IDictionary<string, Type> CustomFormFieldValidators
	  {
		  get
		  {
			return customFormFieldValidators;
		  }
		  set
		  {
			this.customFormFieldValidators = value;
		  }
	  }


	  public virtual FormValidators FormValidators
	  {
		  set
		  {
			this.formValidators = value;
		  }
		  get
		  {
			return formValidators;
		  }
	  }


	  public virtual bool ExecutionTreePrefetchEnabled
	  {
		  get
		  {
			return isExecutionTreePrefetchEnabled;
		  }
		  set
		  {
			this.isExecutionTreePrefetchEnabled = value;
		  }
	  }


	  public virtual ProcessEngineImpl ProcessEngine
	  {
		  get
		  {
			return processEngine;
		  }
	  }

	  /// <summary>
	  /// If set to true, the process engine will save all script variables (created from Java Script, Groovy ...)
	  /// as process variables.
	  /// </summary>
	  public virtual bool AutoStoreScriptVariables
	  {
		  set
		  {
			this.autoStoreScriptVariables = value;
		  }
		  get
		  {
			return autoStoreScriptVariables;
		  }
	  }


	  /// <summary>
	  /// If set to true, the process engine will attempt to pre-compile script sources at runtime
	  /// to optimize script task execution performance.
	  /// </summary>
	  public virtual bool EnableScriptCompilation
	  {
		  set
		  {
			this.enableScriptCompilation = value;
		  }
		  get
		  {
			return enableScriptCompilation;
		  }
	  }


	  public virtual bool EnableGracefulDegradationOnContextSwitchFailure
	  {
		  get
		  {
			return enableGracefulDegradationOnContextSwitchFailure;
		  }
		  set
		  {
			this.enableGracefulDegradationOnContextSwitchFailure = value;
		  }
	  }


	  /// <returns> true if the process engine acquires an exclusive lock when creating a deployment. </returns>
	  public virtual bool DeploymentLockUsed
	  {
		  get
		  {
			return isDeploymentLockUsed;
		  }
		  set
		  {
			this.isDeploymentLockUsed = value;
		  }
	  }


	  /// <returns> true if deployment processing must be synchronized </returns>
	  public virtual bool DeploymentSynchronized
	  {
		  get
		  {
			return isDeploymentSynchronized;
		  }
		  set
		  {
			isDeploymentSynchronized = value;
		  }
	  }


	  public virtual bool CmmnEnabled
	  {
		  get
		  {
			return cmmnEnabled;
		  }
		  set
		  {
			this.cmmnEnabled = value;
		  }
	  }


	  public virtual bool DmnEnabled
	  {
		  get
		  {
			return dmnEnabled;
		  }
		  set
		  {
			this.dmnEnabled = value;
		  }
	  }


	  public virtual ScriptFactory ScriptFactory
	  {
		  get
		  {
			return scriptFactory;
		  }
		  set
		  {
			this.scriptFactory = value;
		  }
	  }

	  public virtual ScriptingEnvironment ScriptingEnvironment
	  {
		  get
		  {
			return scriptingEnvironment;
		  }
		  set
		  {
			this.scriptingEnvironment = value;
		  }
	  }



	  public virtual IList<ScriptEnvResolver> EnvScriptResolvers
	  {
		  get
		  {
			return scriptEnvResolvers;
		  }
		  set
		  {
			this.scriptEnvResolvers = value;
		  }
	  }


	  public virtual ProcessEngineConfiguration setArtifactFactory(ArtifactFactory artifactFactory)
	  {
		this.artifactFactory = artifactFactory;
		return this;
	  }

	  public virtual ArtifactFactory ArtifactFactory
	  {
		  get
		  {
			return artifactFactory;
		  }
	  }

	  public virtual string DefaultSerializationFormat
	  {
		  get
		  {
			return defaultSerializationFormat;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setDefaultSerializationFormat(string defaultSerializationFormat)
	  {
		this.defaultSerializationFormat = defaultSerializationFormat;
		return this;
	  }

	  public virtual bool JavaSerializationFormatEnabled
	  {
		  get
		  {
			return javaSerializationFormatEnabled;
		  }
		  set
		  {
			this.javaSerializationFormatEnabled = value;
		  }
	  }


	  public virtual ProcessEngineConfigurationImpl setDefaultCharsetName(string defaultCharsetName)
	  {
		this.defaultCharsetName = defaultCharsetName;
		return this;
	  }

	  public virtual ProcessEngineConfigurationImpl setDefaultCharset(Charset defautlCharset)
	  {
		this.defaultCharset = defautlCharset;
		return this;
	  }

	  public virtual Charset DefaultCharset
	  {
		  get
		  {
			return defaultCharset;
		  }
	  }

	  public virtual bool DbEntityCacheReuseEnabled
	  {
		  get
		  {
			return isDbEntityCacheReuseEnabled;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setDbEntityCacheReuseEnabled(bool isDbEntityCacheReuseEnabled)
	  {
		this.isDbEntityCacheReuseEnabled = isDbEntityCacheReuseEnabled;
		return this;
	  }

	  public virtual DbEntityCacheKeyMapping DbEntityCacheKeyMapping
	  {
		  get
		  {
			return dbEntityCacheKeyMapping;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setDbEntityCacheKeyMapping(DbEntityCacheKeyMapping dbEntityCacheKeyMapping)
	  {
		this.dbEntityCacheKeyMapping = dbEntityCacheKeyMapping;
		return this;
	  }

	  public virtual ProcessEngineConfigurationImpl setCustomHistoryLevels(IList<HistoryLevel> customHistoryLevels)
	  {
		this.customHistoryLevels = customHistoryLevels;
		return this;
	  }

	  public virtual IList<HistoryLevel> HistoryLevels
	  {
		  get
		  {
			return historyLevels;
		  }
	  }

	  public virtual IList<HistoryLevel> CustomHistoryLevels
	  {
		  get
		  {
			return customHistoryLevels;
		  }
	  }

	  public virtual bool InvokeCustomVariableListeners
	  {
		  get
		  {
			return isInvokeCustomVariableListeners;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setInvokeCustomVariableListeners(bool isInvokeCustomVariableListeners)
	  {
		this.isInvokeCustomVariableListeners = isInvokeCustomVariableListeners;
		return this;
	  }

	  public virtual void close()
	  {
		if (forceCloseMybatisConnectionPool && dataSource is PooledDataSource)
		{

		  // ACT-233: connection pool of Ibatis is not properely initialized if this is not called!
		  ((PooledDataSource) dataSource).forceCloseAll();
		}
	  }

	  public virtual MetricsRegistry MetricsRegistry
	  {
		  get
		  {
			return metricsRegistry;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setMetricsRegistry(MetricsRegistry metricsRegistry)
	  {
		this.metricsRegistry = metricsRegistry;
		return this;
	  }

	  public virtual ProcessEngineConfigurationImpl setMetricsEnabled(bool isMetricsEnabled)
	  {
		this.isMetricsEnabled = isMetricsEnabled;
		return this;
	  }

	  public virtual bool MetricsEnabled
	  {
		  get
		  {
			return isMetricsEnabled;
		  }
	  }

	  public virtual DbMetricsReporter DbMetricsReporter
	  {
		  get
		  {
			return dbMetricsReporter;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setDbMetricsReporter(DbMetricsReporter dbMetricsReporter)
	  {
		this.dbMetricsReporter = dbMetricsReporter;
		return this;
	  }

	  public virtual bool DbMetricsReporterActivate
	  {
		  get
		  {
			return isDbMetricsReporterActivate;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setDbMetricsReporterActivate(bool isDbMetricsReporterEnabled)
	  {
		this.isDbMetricsReporterActivate = isDbMetricsReporterEnabled;
		return this;
	  }

	  public virtual MetricsReporterIdProvider MetricsReporterIdProvider
	  {
		  get
		  {
			return metricsReporterIdProvider;
		  }
		  set
		  {
			this.metricsReporterIdProvider = value;
		  }
	  }


	  public virtual bool EnableScriptEngineCaching
	  {
		  get
		  {
			return enableScriptEngineCaching;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setEnableScriptEngineCaching(bool enableScriptEngineCaching)
	  {
		this.enableScriptEngineCaching = enableScriptEngineCaching;
		return this;
	  }

	  public virtual bool EnableFetchScriptEngineFromProcessApplication
	  {
		  get
		  {
			return enableFetchScriptEngineFromProcessApplication;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setEnableFetchScriptEngineFromProcessApplication(bool enable)
	  {
		this.enableFetchScriptEngineFromProcessApplication = enable;
		return this;
	  }

	  public virtual bool EnableExpressionsInAdhocQueries
	  {
		  get
		  {
			return enableExpressionsInAdhocQueries;
		  }
		  set
		  {
			this.enableExpressionsInAdhocQueries = value;
		  }
	  }


	  public virtual bool EnableExpressionsInStoredQueries
	  {
		  get
		  {
			return enableExpressionsInStoredQueries;
		  }
		  set
		  {
			this.enableExpressionsInStoredQueries = value;
		  }
	  }


	  public virtual bool EnableXxeProcessing
	  {
		  get
		  {
			return enableXxeProcessing;
		  }
		  set
		  {
			this.enableXxeProcessing = value;
		  }
	  }


	  public virtual ProcessEngineConfigurationImpl setBpmnStacktraceVerbose(bool isBpmnStacktraceVerbose)
	  {
		this.isBpmnStacktraceVerbose = isBpmnStacktraceVerbose;
		return this;
	  }

	  public virtual bool BpmnStacktraceVerbose
	  {
		  get
		  {
			return this.isBpmnStacktraceVerbose;
		  }
	  }

	  public virtual bool ForceCloseMybatisConnectionPool
	  {
		  get
		  {
			return forceCloseMybatisConnectionPool;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setForceCloseMybatisConnectionPool(bool forceCloseMybatisConnectionPool)
	  {
		this.forceCloseMybatisConnectionPool = forceCloseMybatisConnectionPool;
		return this;
	  }

	  public virtual bool RestrictUserOperationLogToAuthenticatedUsers
	  {
		  get
		  {
			return restrictUserOperationLogToAuthenticatedUsers;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setRestrictUserOperationLogToAuthenticatedUsers(bool restrictUserOperationLogToAuthenticatedUsers)
	  {
		this.restrictUserOperationLogToAuthenticatedUsers = restrictUserOperationLogToAuthenticatedUsers;
		return this;
	  }

	  public virtual ProcessEngineConfigurationImpl setTenantIdProvider(TenantIdProvider tenantIdProvider)
	  {
		this.tenantIdProvider = tenantIdProvider;
		return this;
	  }

	  public virtual TenantIdProvider TenantIdProvider
	  {
		  get
		  {
			return this.tenantIdProvider;
		  }
	  }

	  public virtual MigrationActivityMatcher MigrationActivityMatcher
	  {
		  set
		  {
			this.migrationActivityMatcher = value;
		  }
		  get
		  {
			return migrationActivityMatcher;
		  }
	  }



	  public virtual IList<MigrationActivityValidator> CustomPreMigrationActivityValidators
	  {
		  set
		  {
			this.customPreMigrationActivityValidators = value;
		  }
		  get
		  {
			return customPreMigrationActivityValidators;
		  }
	  }


	  public virtual IList<MigrationActivityValidator> CustomPostMigrationActivityValidators
	  {
		  set
		  {
			this.customPostMigrationActivityValidators = value;
		  }
		  get
		  {
			return customPostMigrationActivityValidators;
		  }
	  }


	  public virtual IList<MigrationActivityValidator> DefaultMigrationActivityValidators
	  {
		  get
		  {
			IList<MigrationActivityValidator> migrationActivityValidators = new List<MigrationActivityValidator>();
			migrationActivityValidators.Add(SupportedActivityValidator.INSTANCE);
			migrationActivityValidators.Add(SupportedPassiveEventTriggerActivityValidator.INSTANCE);
			migrationActivityValidators.Add(NoCompensationHandlerActivityValidator.INSTANCE);
			return migrationActivityValidators;
		  }
	  }

	  public virtual MigrationInstructionGenerator MigrationInstructionGenerator
	  {
		  set
		  {
			this.migrationInstructionGenerator = value;
		  }
		  get
		  {
			return migrationInstructionGenerator;
		  }
	  }


	  public virtual IList<MigrationInstructionValidator> MigrationInstructionValidators
	  {
		  set
		  {
			this.migrationInstructionValidators = value;
		  }
		  get
		  {
			return migrationInstructionValidators;
		  }
	  }


	  public virtual IList<MigrationInstructionValidator> CustomPostMigrationInstructionValidators
	  {
		  set
		  {
			this.customPostMigrationInstructionValidators = value;
		  }
		  get
		  {
			return customPostMigrationInstructionValidators;
		  }
	  }


	  public virtual IList<MigrationInstructionValidator> CustomPreMigrationInstructionValidators
	  {
		  set
		  {
			this.customPreMigrationInstructionValidators = value;
		  }
		  get
		  {
			return customPreMigrationInstructionValidators;
    
		  }
	  }


	  public virtual IList<MigrationInstructionValidator> DefaultMigrationInstructionValidators
	  {
		  get
		  {
			IList<MigrationInstructionValidator> migrationInstructionValidators = new List<MigrationInstructionValidator>();
			migrationInstructionValidators.Add(new SameBehaviorInstructionValidator());
			migrationInstructionValidators.Add(new SameEventTypeValidator());
			migrationInstructionValidators.Add(new OnlyOnceMappedActivityInstructionValidator());
			migrationInstructionValidators.Add(new CannotAddMultiInstanceBodyValidator());
			migrationInstructionValidators.Add(new CannotAddMultiInstanceInnerActivityValidator());
			migrationInstructionValidators.Add(new CannotRemoveMultiInstanceInnerActivityValidator());
			migrationInstructionValidators.Add(new GatewayMappingValidator());
			migrationInstructionValidators.Add(new SameEventScopeInstructionValidator());
			migrationInstructionValidators.Add(new UpdateEventTriggersValidator());
			migrationInstructionValidators.Add(new AdditionalFlowScopeInstructionValidator());
			migrationInstructionValidators.Add(new ConditionalEventUpdateEventTriggerValidator());
			return migrationInstructionValidators;
		  }
	  }

	  public virtual IList<MigratingActivityInstanceValidator> MigratingActivityInstanceValidators
	  {
		  set
		  {
			this.migratingActivityInstanceValidators = value;
		  }
		  get
		  {
			return migratingActivityInstanceValidators;
		  }
	  }


	  public virtual IList<MigratingActivityInstanceValidator> CustomPostMigratingActivityInstanceValidators
	  {
		  set
		  {
			this.customPostMigratingActivityInstanceValidators = value;
		  }
		  get
		  {
			return customPostMigratingActivityInstanceValidators;
		  }
	  }


	  public virtual IList<MigratingActivityInstanceValidator> CustomPreMigratingActivityInstanceValidators
	  {
		  set
		  {
			this.customPreMigratingActivityInstanceValidators = value;
		  }
		  get
		  {
			return customPreMigratingActivityInstanceValidators;
		  }
	  }


	  public virtual IList<MigratingTransitionInstanceValidator> MigratingTransitionInstanceValidators
	  {
		  get
		  {
			return migratingTransitionInstanceValidators;
		  }
	  }

	  public virtual IList<MigratingCompensationInstanceValidator> MigratingCompensationInstanceValidators
	  {
		  get
		  {
			return migratingCompensationInstanceValidators;
		  }
	  }

	  public virtual IList<MigratingActivityInstanceValidator> DefaultMigratingActivityInstanceValidators
	  {
		  get
		  {
			IList<MigratingActivityInstanceValidator> migratingActivityInstanceValidators = new List<MigratingActivityInstanceValidator>();
    
			migratingActivityInstanceValidators.Add(new NoUnmappedLeafInstanceValidator());
			migratingActivityInstanceValidators.Add(new VariableConflictActivityInstanceValidator());
			migratingActivityInstanceValidators.Add(new SupportedActivityInstanceValidator());
    
			return migratingActivityInstanceValidators;
		  }
	  }

	  public virtual IList<MigratingTransitionInstanceValidator> DefaultMigratingTransitionInstanceValidators
	  {
		  get
		  {
			IList<MigratingTransitionInstanceValidator> migratingTransitionInstanceValidators = new List<MigratingTransitionInstanceValidator>();
    
			migratingTransitionInstanceValidators.Add(new NoUnmappedLeafInstanceValidator());
			migratingTransitionInstanceValidators.Add(new AsyncAfterMigrationValidator());
			migratingTransitionInstanceValidators.Add(new AsyncProcessStartMigrationValidator());
			migratingTransitionInstanceValidators.Add(new AsyncMigrationValidator());
    
			return migratingTransitionInstanceValidators;
		  }
	  }

	  public virtual IList<CommandChecker> CommandCheckers
	  {
		  get
		  {
			return commandCheckers;
		  }
		  set
		  {
			this.commandCheckers = value;
		  }
	  }


	  public virtual ProcessEngineConfigurationImpl setUseSharedSqlSessionFactory(bool isUseSharedSqlSessionFactory)
	  {
		this.isUseSharedSqlSessionFactory = isUseSharedSqlSessionFactory;
		return this;
	  }

	  public virtual bool UseSharedSqlSessionFactory
	  {
		  get
		  {
			return isUseSharedSqlSessionFactory;
		  }
	  }

	  public virtual bool DisableStrictCallActivityValidation
	  {
		  get
		  {
			return disableStrictCallActivityValidation;
		  }
		  set
		  {
			this.disableStrictCallActivityValidation = value;
		  }
	  }


	  public virtual string HistoryCleanupBatchWindowStartTime
	  {
		  get
		  {
			return historyCleanupBatchWindowStartTime;
		  }
		  set
		  {
			this.historyCleanupBatchWindowStartTime = value;
		  }
	  }


	  public virtual string HistoryCleanupBatchWindowEndTime
	  {
		  get
		  {
			return historyCleanupBatchWindowEndTime;
		  }
		  set
		  {
			this.historyCleanupBatchWindowEndTime = value;
		  }
	  }


	  public virtual string MondayHistoryCleanupBatchWindowStartTime
	  {
		  get
		  {
			return mondayHistoryCleanupBatchWindowStartTime;
		  }
		  set
		  {
			this.mondayHistoryCleanupBatchWindowStartTime = value;
		  }
	  }


	  public virtual string MondayHistoryCleanupBatchWindowEndTime
	  {
		  get
		  {
			return mondayHistoryCleanupBatchWindowEndTime;
		  }
		  set
		  {
			this.mondayHistoryCleanupBatchWindowEndTime = value;
		  }
	  }


	  public virtual string TuesdayHistoryCleanupBatchWindowStartTime
	  {
		  get
		  {
			return tuesdayHistoryCleanupBatchWindowStartTime;
		  }
		  set
		  {
			this.tuesdayHistoryCleanupBatchWindowStartTime = value;
		  }
	  }


	  public virtual string TuesdayHistoryCleanupBatchWindowEndTime
	  {
		  get
		  {
			return tuesdayHistoryCleanupBatchWindowEndTime;
		  }
		  set
		  {
			this.tuesdayHistoryCleanupBatchWindowEndTime = value;
		  }
	  }


	  public virtual string WednesdayHistoryCleanupBatchWindowStartTime
	  {
		  get
		  {
			return wednesdayHistoryCleanupBatchWindowStartTime;
		  }
		  set
		  {
			this.wednesdayHistoryCleanupBatchWindowStartTime = value;
		  }
	  }


	  public virtual string WednesdayHistoryCleanupBatchWindowEndTime
	  {
		  get
		  {
			return wednesdayHistoryCleanupBatchWindowEndTime;
		  }
		  set
		  {
			this.wednesdayHistoryCleanupBatchWindowEndTime = value;
		  }
	  }


	  public virtual string ThursdayHistoryCleanupBatchWindowStartTime
	  {
		  get
		  {
			return thursdayHistoryCleanupBatchWindowStartTime;
		  }
		  set
		  {
			this.thursdayHistoryCleanupBatchWindowStartTime = value;
		  }
	  }


	  public virtual string ThursdayHistoryCleanupBatchWindowEndTime
	  {
		  get
		  {
			return thursdayHistoryCleanupBatchWindowEndTime;
		  }
		  set
		  {
			this.thursdayHistoryCleanupBatchWindowEndTime = value;
		  }
	  }


	  public virtual string FridayHistoryCleanupBatchWindowStartTime
	  {
		  get
		  {
			return fridayHistoryCleanupBatchWindowStartTime;
		  }
		  set
		  {
			this.fridayHistoryCleanupBatchWindowStartTime = value;
		  }
	  }


	  public virtual string FridayHistoryCleanupBatchWindowEndTime
	  {
		  get
		  {
			return fridayHistoryCleanupBatchWindowEndTime;
		  }
		  set
		  {
			this.fridayHistoryCleanupBatchWindowEndTime = value;
		  }
	  }


	  public virtual string SaturdayHistoryCleanupBatchWindowStartTime
	  {
		  get
		  {
			return saturdayHistoryCleanupBatchWindowStartTime;
		  }
		  set
		  {
			this.saturdayHistoryCleanupBatchWindowStartTime = value;
		  }
	  }


	  public virtual string SaturdayHistoryCleanupBatchWindowEndTime
	  {
		  get
		  {
			return saturdayHistoryCleanupBatchWindowEndTime;
		  }
		  set
		  {
			this.saturdayHistoryCleanupBatchWindowEndTime = value;
		  }
	  }


	  public virtual string SundayHistoryCleanupBatchWindowStartTime
	  {
		  get
		  {
			return sundayHistoryCleanupBatchWindowStartTime;
		  }
		  set
		  {
			this.sundayHistoryCleanupBatchWindowStartTime = value;
		  }
	  }


	  public virtual string SundayHistoryCleanupBatchWindowEndTime
	  {
		  get
		  {
			return sundayHistoryCleanupBatchWindowEndTime;
		  }
		  set
		  {
			this.sundayHistoryCleanupBatchWindowEndTime = value;
		  }
	  }


	  public virtual DateTime HistoryCleanupBatchWindowStartTimeAsDate
	  {
		  get
		  {
			return historyCleanupBatchWindowStartTimeAsDate;
		  }
		  set
		  {
			this.historyCleanupBatchWindowStartTimeAsDate = value;
		  }
	  }


	  public virtual DateTime HistoryCleanupBatchWindowEndTimeAsDate
	  {
		  set
		  {
			this.historyCleanupBatchWindowEndTimeAsDate = value;
		  }
		  get
		  {
			return historyCleanupBatchWindowEndTimeAsDate;
		  }
	  }


	  public virtual IDictionary<int, BatchWindowConfiguration> HistoryCleanupBatchWindows
	  {
		  get
		  {
			return historyCleanupBatchWindows;
		  }
		  set
		  {
			this.historyCleanupBatchWindows = value;
		  }
	  }


	  public virtual int HistoryCleanupBatchSize
	  {
		  get
		  {
			return historyCleanupBatchSize;
		  }
		  set
		  {
			this.historyCleanupBatchSize = value;
		  }
	  }


	  public virtual int HistoryCleanupBatchThreshold
	  {
		  get
		  {
			return historyCleanupBatchThreshold;
		  }
		  set
		  {
			this.historyCleanupBatchThreshold = value;
		  }
	  }


	  public virtual bool HistoryCleanupMetricsEnabled
	  {
		  get
		  {
			return historyCleanupMetricsEnabled;
		  }
		  set
		  {
			this.historyCleanupMetricsEnabled = value;
		  }
	  }


	  public virtual string HistoryTimeToLive
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


	  public virtual string BatchOperationHistoryTimeToLive
	  {
		  get
		  {
			return batchOperationHistoryTimeToLive;
		  }
		  set
		  {
			this.batchOperationHistoryTimeToLive = value;
		  }
	  }

	  public virtual int HistoryCleanupDegreeOfParallelism
	  {
		  get
		  {
			return historyCleanupDegreeOfParallelism;
		  }
		  set
		  {
			this.historyCleanupDegreeOfParallelism = value;
		  }
	  }



	  public virtual IDictionary<string, string> BatchOperationsForHistoryCleanup
	  {
		  get
		  {
			return batchOperationsForHistoryCleanup;
		  }
		  set
		  {
			this.batchOperationsForHistoryCleanup = value;
		  }
	  }


	  public virtual IDictionary<string, int> ParsedBatchOperationsForHistoryCleanup
	  {
		  get
		  {
			return parsedBatchOperationsForHistoryCleanup;
		  }
		  set
		  {
			this.parsedBatchOperationsForHistoryCleanup = value;
		  }
	  }


	  public virtual BatchWindowManager BatchWindowManager
	  {
		  get
		  {
			return batchWindowManager;
		  }
		  set
		  {
			this.batchWindowManager = value;
		  }
	  }


	  public virtual HistoryRemovalTimeProvider HistoryRemovalTimeProvider
	  {
		  get
		  {
			return historyRemovalTimeProvider;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setHistoryRemovalTimeProvider(HistoryRemovalTimeProvider removalTimeProvider)
	  {
		historyRemovalTimeProvider = removalTimeProvider;
		return this;
	  }

	  public virtual string HistoryRemovalTimeStrategy
	  {
		  get
		  {
			return historyRemovalTimeStrategy;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setHistoryRemovalTimeStrategy(string removalTimeStrategy)
	  {
		historyRemovalTimeStrategy = removalTimeStrategy;
		return this;
	  }

	  public virtual string HistoryCleanupStrategy
	  {
		  get
		  {
			return historyCleanupStrategy;
		  }
	  }

	  public virtual ProcessEngineConfigurationImpl setHistoryCleanupStrategy(string historyCleanupStrategy)
	  {
		this.historyCleanupStrategy = historyCleanupStrategy;
		return this;
	  }

	  public virtual int FailedJobListenerMaxRetries
	  {
		  get
		  {
			return failedJobListenerMaxRetries;
		  }
		  set
		  {
			this.failedJobListenerMaxRetries = value;
		  }
	  }


	  public virtual string FailedJobRetryTimeCycle
	  {
		  get
		  {
			return failedJobRetryTimeCycle;
		  }
		  set
		  {
			this.failedJobRetryTimeCycle = value;
		  }
	  }


	  public virtual int LoginMaxAttempts
	  {
		  get
		  {
			return loginMaxAttempts;
		  }
		  set
		  {
			this.loginMaxAttempts = value;
		  }
	  }


	  public virtual int LoginDelayFactor
	  {
		  get
		  {
			return loginDelayFactor;
		  }
		  set
		  {
			this.loginDelayFactor = value;
		  }
	  }


	  public virtual int LoginDelayMaxTime
	  {
		  get
		  {
			return loginDelayMaxTime;
		  }
		  set
		  {
			this.loginDelayMaxTime = value;
		  }
	  }


	  public virtual int LoginDelayBase
	  {
		  get
		  {
			return loginDelayBase;
		  }
		  set
		  {
			this.loginDelayBase = value;
		  }
	  }


	  public virtual IList<string> AdminGroups
	  {
		  get
		  {
			return adminGroups;
		  }
		  set
		  {
			this.adminGroups = value;
		  }
	  }


	  public virtual IList<string> AdminUsers
	  {
		  get
		  {
			return adminUsers;
		  }
		  set
		  {
			this.adminUsers = value;
		  }
	  }


	}

}