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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using RowBounds = org.apache.ibatis.session.RowBounds;
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricCaseActivityInstance = org.camunda.bpm.engine.history.HistoricCaseActivityInstance;
	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricFormProperty = org.camunda.bpm.engine.history.HistoricFormProperty;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using HistoricBatchEntity = org.camunda.bpm.engine.impl.batch.history.HistoricBatchEntity;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using CaseSentryPartEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseSentryPartEntity;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using EnginePersistenceLogger = org.camunda.bpm.engine.impl.db.EnginePersistenceLogger;
	using DbSqlSessionFactory = org.camunda.bpm.engine.impl.db.sql.DbSqlSessionFactory;
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using DecisionRequirementsDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionEntity;
	using org.camunda.bpm.engine.impl.history.@event;
	using TableMetaData = org.camunda.bpm.engine.management.TableMetaData;
	using TablePage = org.camunda.bpm.engine.management.TablePage;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class TableDataManager : AbstractManager
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  public static IDictionary<Type, string> apiTypeToTableNameMap = new Dictionary<Type, string>();
	  public static IDictionary<Type, string> persistentObjectToTableNameMap = new Dictionary<Type, string>();

	  static TableDataManager()
	  {
		// runtime
		persistentObjectToTableNameMap[typeof(TaskEntity)] = "ACT_RU_TASK";
		persistentObjectToTableNameMap[typeof(ExternalTaskEntity)] = "ACT_RU_EXT_TASK";
		persistentObjectToTableNameMap[typeof(ExecutionEntity)] = "ACT_RU_EXECUTION";
		persistentObjectToTableNameMap[typeof(IdentityLinkEntity)] = "ACT_RU_IDENTITYLINK";
		persistentObjectToTableNameMap[typeof(VariableInstanceEntity)] = "ACT_RU_VARIABLE";

		persistentObjectToTableNameMap[typeof(JobEntity)] = "ACT_RU_JOB";
		persistentObjectToTableNameMap[typeof(MessageEntity)] = "ACT_RU_JOB";
		persistentObjectToTableNameMap[typeof(TimerEntity)] = "ACT_RU_JOB";
		persistentObjectToTableNameMap[typeof(JobDefinitionEntity)] = "ACT_RU_JOBDEF";
		persistentObjectToTableNameMap[typeof(BatchEntity)] = "ACT_RU_BATCH";

		persistentObjectToTableNameMap[typeof(IncidentEntity)] = "ACT_RU_INCIDENT";

		persistentObjectToTableNameMap[typeof(EventSubscriptionEntity)] = "ACT_RU_EVENT_SUBSCR";

		persistentObjectToTableNameMap[typeof(FilterEntity)] = "ACT_RU_FILTER";

		persistentObjectToTableNameMap[typeof(MeterLogEntity)] = "ACT_RU_METER_LOG";
		// repository
		persistentObjectToTableNameMap[typeof(DeploymentEntity)] = "ACT_RE_DEPLOYMENT";
		persistentObjectToTableNameMap[typeof(ProcessDefinitionEntity)] = "ACT_RE_PROCDEF";

		// CMMN
		persistentObjectToTableNameMap[typeof(CaseDefinitionEntity)] = "ACT_RE_CASE_DEF";
		persistentObjectToTableNameMap[typeof(CaseExecutionEntity)] = "ACT_RU_CASE_EXECUTION";
		persistentObjectToTableNameMap[typeof(CaseSentryPartEntity)] = "ACT_RU_CASE_SENTRY_PART";

		// DMN
		persistentObjectToTableNameMap[typeof(DecisionRequirementsDefinitionEntity)] = "ACT_RE_DECISION_REQ_DEF";
		persistentObjectToTableNameMap[typeof(DecisionDefinitionEntity)] = "ACT_RE_DECISION_DEF";
		persistentObjectToTableNameMap[typeof(HistoricDecisionInputInstanceEntity)] = "ACT_HI_DEC_IN";
		persistentObjectToTableNameMap[typeof(HistoricDecisionOutputInstanceEntity)] = "ACT_HI_DEC_OUT";

		// history
		persistentObjectToTableNameMap[typeof(CommentEntity)] = "ACT_HI_COMMENT";

		persistentObjectToTableNameMap[typeof(HistoricActivityInstanceEntity)] = "ACT_HI_ACTINST";
		persistentObjectToTableNameMap[typeof(AttachmentEntity)] = "ACT_HI_ATTACHMENT";
		persistentObjectToTableNameMap[typeof(HistoricProcessInstanceEntity)] = "ACT_HI_PROCINST";
		persistentObjectToTableNameMap[typeof(HistoricTaskInstanceEntity)] = "ACT_HI_TASKINST";
		persistentObjectToTableNameMap[typeof(HistoricJobLogEventEntity)] = "ACT_HI_JOB_LOG";
		persistentObjectToTableNameMap[typeof(HistoricIncidentEventEntity)] = "ACT_HI_INCIDENT";
		persistentObjectToTableNameMap[typeof(HistoricBatchEntity)] = "ACT_HI_BATCH";
		persistentObjectToTableNameMap[typeof(HistoricExternalTaskLogEntity)] = "ACT_HI_EXT_TASK_LOG";

		persistentObjectToTableNameMap[typeof(HistoricCaseInstanceEntity)] = "ACT_HI_CASEINST";
		persistentObjectToTableNameMap[typeof(HistoricCaseActivityInstanceEntity)] = "ACT_HI_CASEACTINST";
		persistentObjectToTableNameMap[typeof(HistoricIdentityLinkLogEntity)] = "ACT_HI_IDENTITYLINK";
		// a couple of stuff goes to the same table
		persistentObjectToTableNameMap[typeof(HistoricFormPropertyEntity)] = "ACT_HI_DETAIL";
		persistentObjectToTableNameMap[typeof(HistoricVariableInstanceEntity)] = "ACT_HI_DETAIL";
		persistentObjectToTableNameMap[typeof(HistoricVariableInstanceEntity)] = "ACT_HI_VARINST";
		persistentObjectToTableNameMap[typeof(HistoricDetailEventEntity)] = "ACT_HI_DETAIL";

		persistentObjectToTableNameMap[typeof(HistoricDecisionInstanceEntity)] = "ACT_HI_DECINST";
		persistentObjectToTableNameMap[typeof(UserOperationLogEntryEventEntity)] = "ACT_HI_OP_LOG";


		// Identity module
		persistentObjectToTableNameMap[typeof(GroupEntity)] = "ACT_ID_GROUP";
		persistentObjectToTableNameMap[typeof(MembershipEntity)] = "ACT_ID_MEMBERSHIP";
		persistentObjectToTableNameMap[typeof(TenantEntity)] = "ACT_ID_TENANT";
		persistentObjectToTableNameMap[typeof(TenantMembershipEntity)] = "ACT_ID_TENANT_MEMBER";
		persistentObjectToTableNameMap[typeof(UserEntity)] = "ACT_ID_USER";
		persistentObjectToTableNameMap[typeof(IdentityInfoEntity)] = "ACT_ID_INFO";
		persistentObjectToTableNameMap[typeof(AuthorizationEntity)] = "ACT_RU_AUTHORIZATION";


		// general
		persistentObjectToTableNameMap[typeof(PropertyEntity)] = "ACT_GE_PROPERTY";
		persistentObjectToTableNameMap[typeof(ByteArrayEntity)] = "ACT_GE_BYTEARRAY";
		persistentObjectToTableNameMap[typeof(ResourceEntity)] = "ACT_GE_BYTEARRAY";
		persistentObjectToTableNameMap[typeof(SchemaLogEntryEntity)] = "ACT_GE_SCHEMA_LOG";
		persistentObjectToTableNameMap[typeof(FilterEntity)] = "ACT_RU_FILTER";

		// and now the map for the API types (does not cover all cases)
		apiTypeToTableNameMap[typeof(Task)] = "ACT_RU_TASK";
		apiTypeToTableNameMap[typeof(Execution)] = "ACT_RU_EXECUTION";
		apiTypeToTableNameMap[typeof(ProcessInstance)] = "ACT_RU_EXECUTION";
		apiTypeToTableNameMap[typeof(ProcessDefinition)] = "ACT_RE_PROCDEF";
		apiTypeToTableNameMap[typeof(Deployment)] = "ACT_RE_DEPLOYMENT";
		apiTypeToTableNameMap[typeof(Job)] = "ACT_RU_JOB";
		apiTypeToTableNameMap[typeof(Incident)] = "ACT_RU_INCIDENT";
		apiTypeToTableNameMap[typeof(Filter)] = "ACT_RU_FILTER";


		// history
		apiTypeToTableNameMap[typeof(HistoricProcessInstance)] = "ACT_HI_PROCINST";
		apiTypeToTableNameMap[typeof(HistoricActivityInstance)] = "ACT_HI_ACTINST";
		apiTypeToTableNameMap[typeof(HistoricDetail)] = "ACT_HI_DETAIL";
		apiTypeToTableNameMap[typeof(HistoricVariableUpdate)] = "ACT_HI_DETAIL";
		apiTypeToTableNameMap[typeof(HistoricFormProperty)] = "ACT_HI_DETAIL";
		apiTypeToTableNameMap[typeof(HistoricTaskInstance)] = "ACT_HI_TASKINST";
		apiTypeToTableNameMap[typeof(HistoricVariableInstance)] = "ACT_HI_VARINST";


		apiTypeToTableNameMap[typeof(HistoricCaseInstance)] = "ACT_HI_CASEINST";
		apiTypeToTableNameMap[typeof(HistoricCaseActivityInstance)] = "ACT_HI_CASEACTINST";

		apiTypeToTableNameMap[typeof(HistoricDecisionInstance)] = "ACT_HI_DECINST";

		// TODO: Identity skipped for the moment as no SQL injection is provided here
	  }

	  public virtual IDictionary<string, long> TableCount
	  {
		  get
		  {
			IDictionary<string, long> tableCount = new Dictionary<string, long>();
			try
			{
			  foreach (string tableName in DbEntityManager.TableNamesPresentInDatabase)
			  {
				tableCount[tableName] = getTableCount(tableName);
			  }
			  LOG.countRowsPerProcessEngineTable(tableCount);
			}
			catch (Exception e)
			{
			  throw LOG.countTableRowsException(e);
			}
			return tableCount;
		  }
	  }

	  protected internal virtual long getTableCount(string tableName)
	  {
		LOG.selectTableCountForTable(tableName);
		long? count = (long?) DbEntityManager.selectOne("selectTableCount", Collections.singletonMap("tableName", tableName));
		return count.Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public org.camunda.bpm.engine.management.TablePage getTablePage(org.camunda.bpm.engine.impl.TablePageQueryImpl tablePageQuery, int firstResult, int maxResults)
	  public virtual TablePage getTablePage(TablePageQueryImpl tablePageQuery, int firstResult, int maxResults)
	  {

		TablePage tablePage = new TablePage();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") List tableData = getDbSqlSession().getSqlSession().selectList("selectTableData", tablePageQuery, new org.apache.ibatis.session.RowBounds(firstResult, maxResults));
		System.Collections.IList tableData = DbSqlSession.SqlSession.selectList("selectTableData", tablePageQuery, new RowBounds(firstResult, maxResults));

		tablePage.TableName = tablePageQuery.TableName;
		tablePage.Total = getTableCount(tablePageQuery.TableName);
		tablePage.Rows = tableData;
		tablePage.FirstResult = firstResult;

		return tablePage;
	  }

	  public virtual IList<Type> getEntities(string tableName)
	  {
		string databaseTablePrefix = DbSqlSession.DbSqlSessionFactory.DatabaseTablePrefix;
		IList<Type> entities = new List<Type>();

		ISet<Type> entityClasses = persistentObjectToTableNameMap.Keys;
		foreach (Type entityClass in entityClasses)
		{
		  string entityTableName = persistentObjectToTableNameMap[entityClass];
		  if ((databaseTablePrefix + entityTableName).Equals(tableName))
		  {
			entities.Add(entityClass);
		  }
		}
		return entities;
	  }

	  public virtual string getTableName(Type entityClass, bool withPrefix)
	  {
		string databaseTablePrefix = DbSqlSession.DbSqlSessionFactory.DatabaseTablePrefix;
		string tableName = null;

		if (entityClass.IsAssignableFrom(typeof(DbEntity)))
		{
		  tableName = persistentObjectToTableNameMap[entityClass];
		}
		else
		{
		  tableName = apiTypeToTableNameMap[entityClass];
		}
		if (withPrefix)
		{
		  return databaseTablePrefix + tableName;
		}
		else
		{
		  return tableName;
		}
	  }

	  public virtual TableMetaData getTableMetaData(string tableName)
	  {
		TableMetaData result = new TableMetaData();
		ResultSet resultSet = null;

		try
		{
		  try
		  {
			result.TableName = tableName;
			DatabaseMetaData metaData = DbSqlSession.SqlSession.Connection.MetaData;

			if (DbSqlSessionFactory.POSTGRES.Equals(DbSqlSession.DbSqlSessionFactory.DatabaseType))
			{
			  tableName = tableName.ToLower();
			}

			string databaseSchema = DbSqlSession.DbSqlSessionFactory.DatabaseSchema;
			tableName = DbSqlSession.prependDatabaseTablePrefix(tableName);

			resultSet = metaData.getColumns(null, databaseSchema, tableName, null);
			while (resultSet.next())
			{
			  string name = resultSet.getString("COLUMN_NAME").ToUpper();
			  string type = resultSet.getString("TYPE_NAME").ToUpper();
			  result.addColumnMetaData(name, type);
			}

		  }
		  catch (SQLException se)
		  {
			throw se;
		  }
		  finally
		  {
			if (resultSet != null)
			{
			  resultSet.close();
			}
		  }
		}
		catch (Exception e)
		{
		  throw LOG.retrieveMetadataException(e);
		}

		if (result.ColumnNames.Count == 0)
		{
		  // According to API, when a table doesn't exist, null should be returned
		  result = null;
		}
		return result;
	  }

	}

}