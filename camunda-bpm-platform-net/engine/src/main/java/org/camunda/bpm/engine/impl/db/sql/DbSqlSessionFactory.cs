using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

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
namespace org.camunda.bpm.engine.impl.db.sql
{

	using SqlSessionFactory = org.apache.ibatis.session.SqlSessionFactory;
	using IdGenerator = org.camunda.bpm.engine.impl.cfg.IdGenerator;
	using Session = org.camunda.bpm.engine.impl.interceptor.Session;
	using SessionFactory = org.camunda.bpm.engine.impl.interceptor.SessionFactory;
	using ClassNameUtil = org.camunda.bpm.engine.impl.util.ClassNameUtil;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class DbSqlSessionFactory : SessionFactory
	{

	  public const string MSSQL = "mssql";
	  public const string DB2 = "db2";
	  public const string ORACLE = "oracle";
	  public const string H2 = "h2";
	  public const string MYSQL = "mysql";
	  public const string POSTGRES = "postgres";
	  public const string MARIADB = "mariadb";
	  public static readonly string[] SUPPORTED_DATABASES = new string[] {MSSQL, DB2, ORACLE, H2, MYSQL, POSTGRES, MARIADB};

	  protected internal static readonly IDictionary<string, IDictionary<string, string>> databaseSpecificStatements = new Dictionary<string, IDictionary<string, string>>();

	  public static readonly IDictionary<string, string> databaseSpecificLimitBeforeStatements = new Dictionary<string, string>();
	  public static readonly IDictionary<string, string> databaseSpecificLimitAfterStatements = new Dictionary<string, string>();
	  //limit statements that can be used to select first N rows without OFFSET
	  public static readonly IDictionary<string, string> databaseSpecificLimitBeforeWithoutOffsetStatements = new Dictionary<string, string>();
	  public static readonly IDictionary<string, string> databaseSpecificLimitAfterWithoutOffsetStatements = new Dictionary<string, string>();
	  // limitAfter statements that can be used with subqueries
	  public static readonly IDictionary<string, string> databaseSpecificInnerLimitAfterStatements = new Dictionary<string, string>();
	  public static readonly IDictionary<string, string> databaseSpecificLimitBetweenStatements = new Dictionary<string, string>();
	  public static readonly IDictionary<string, string> databaseSpecificLimitBetweenFilterStatements = new Dictionary<string, string>();

	  public static readonly IDictionary<string, string> optimizeDatabaseSpecificLimitBeforeWithoutOffsetStatements = new Dictionary<string, string>();
	  public static readonly IDictionary<string, string> optimizeDatabaseSpecificLimitAfterWithoutOffsetStatements = new Dictionary<string, string>();

	  public static readonly IDictionary<string, string> databaseSpecificEscapeChar = new Dictionary<string, string>();

	  public static readonly IDictionary<string, string> databaseSpecificOrderByStatements = new Dictionary<string, string>();
	  public static readonly IDictionary<string, string> databaseSpecificLimitBeforeNativeQueryStatements = new Dictionary<string, string>();

	  public static readonly IDictionary<string, string> databaseSpecificBitAnd1 = new Dictionary<string, string>();
	  public static readonly IDictionary<string, string> databaseSpecificBitAnd2 = new Dictionary<string, string>();
	  public static readonly IDictionary<string, string> databaseSpecificBitAnd3 = new Dictionary<string, string>();

	  public static readonly IDictionary<string, string> databaseSpecificDatepart1 = new Dictionary<string, string>();
	  public static readonly IDictionary<string, string> databaseSpecificDatepart2 = new Dictionary<string, string>();
	  public static readonly IDictionary<string, string> databaseSpecificDatepart3 = new Dictionary<string, string>();

	  public static readonly IDictionary<string, string> databaseSpecificDummyTable = new Dictionary<string, string>();

	  public static readonly IDictionary<string, string> databaseSpecificIfNull = new Dictionary<string, string>();

	  public static readonly IDictionary<string, string> databaseSpecificTrueConstant = new Dictionary<string, string>();
	  public static readonly IDictionary<string, string> databaseSpecificFalseConstant = new Dictionary<string, string>();

	  public static readonly IDictionary<string, string> databaseSpecificDistinct = new Dictionary<string, string>();

	  public static readonly IDictionary<string, IDictionary<string, string>> dbSpecificConstants = new Dictionary<string, IDictionary<string, string>>();

	  public static readonly IDictionary<string, string> databaseSpecificDaysComparator = new Dictionary<string, string>();

	  public static readonly IDictionary<string, string> databaseSpecificCollationForCaseSensitivity = new Dictionary<string, string>();

	  static DbSqlSessionFactory()
	  {

		string defaultOrderBy = "order by ${internalOrderBy}";

		string defaultEscapeChar = "'\\'";

		// h2
		databaseSpecificLimitBeforeStatements[H2] = "";
		optimizeDatabaseSpecificLimitBeforeWithoutOffsetStatements[H2] = "";
		databaseSpecificLimitAfterStatements[H2] = "LIMIT #{maxResults} OFFSET #{firstResult}";
		optimizeDatabaseSpecificLimitAfterWithoutOffsetStatements[H2] = "LIMIT #{maxResults}";
		databaseSpecificLimitBeforeWithoutOffsetStatements[H2] = "";
		databaseSpecificLimitAfterWithoutOffsetStatements[H2] = "LIMIT #{maxResults}";
		databaseSpecificInnerLimitAfterStatements[H2] = databaseSpecificLimitAfterStatements[H2];
		databaseSpecificLimitBetweenStatements[H2] = "";
		databaseSpecificLimitBetweenFilterStatements[H2] = "";
		databaseSpecificOrderByStatements[H2] = defaultOrderBy;
		databaseSpecificLimitBeforeNativeQueryStatements[H2] = "";
		databaseSpecificDistinct[H2] = "distinct";

		databaseSpecificEscapeChar[H2] = defaultEscapeChar;

		databaseSpecificBitAnd1[H2] = "BITAND(";
		databaseSpecificBitAnd2[H2] = ",";
		databaseSpecificBitAnd3[H2] = ")";
		databaseSpecificDatepart1[H2] = "";
		databaseSpecificDatepart2[H2] = "(";
		databaseSpecificDatepart3[H2] = ")";

		databaseSpecificDummyTable[H2] = "";
		databaseSpecificTrueConstant[H2] = "1";
		databaseSpecificFalseConstant[H2] = "0";
		databaseSpecificIfNull[H2] = "IFNULL";

		databaseSpecificDaysComparator[H2] = "DATEDIFF(DAY, ${date}, #{currentTimestamp}) >= ${days}";

		databaseSpecificCollationForCaseSensitivity[H2] = "";

		Dictionary<string, string> constants = new Dictionary<string, string>();
		constants["constant.event"] = "'event'";
		constants["constant.op_message"] = "NEW_VALUE_ || '_|_' || PROPERTY_";
		constants["constant_for_update"] = "for update";
		constants["constant.datepart.quarter"] = "QUARTER";
		constants["constant.datepart.month"] = "MONTH";
		constants["constant.datepart.minute"] = "MINUTE";
		constants["constant.null.startTime"] = "null START_TIME_";
		constants["constant.varchar.cast"] = "'${key}'";
		constants["constant.null.reporter"] = "NULL AS REPORTER_";
		dbSpecificConstants[H2] = constants;

		// mysql specific
		// use the same specific for mariadb since it based on mysql and work with the exactly same statements
		foreach (string mysqlLikeDatabase in Arrays.asList(MYSQL, MARIADB))
		{

		  databaseSpecificLimitBeforeStatements[mysqlLikeDatabase] = "";
		  optimizeDatabaseSpecificLimitBeforeWithoutOffsetStatements[mysqlLikeDatabase] = "";
		  databaseSpecificLimitAfterStatements[mysqlLikeDatabase] = "LIMIT #{maxResults} OFFSET #{firstResult}";
		  optimizeDatabaseSpecificLimitAfterWithoutOffsetStatements[mysqlLikeDatabase] = "LIMIT #{maxResults}";
		  databaseSpecificLimitBeforeWithoutOffsetStatements[mysqlLikeDatabase] = "";
		  databaseSpecificLimitAfterWithoutOffsetStatements[mysqlLikeDatabase] = "LIMIT #{maxResults}";
		  databaseSpecificInnerLimitAfterStatements[mysqlLikeDatabase] = databaseSpecificLimitAfterStatements[mysqlLikeDatabase];
		  databaseSpecificLimitBetweenStatements[mysqlLikeDatabase] = "";
		  databaseSpecificLimitBetweenFilterStatements[mysqlLikeDatabase] = "";
		  databaseSpecificOrderByStatements[mysqlLikeDatabase] = defaultOrderBy;
		  databaseSpecificLimitBeforeNativeQueryStatements[mysqlLikeDatabase] = "";
		  databaseSpecificDistinct[mysqlLikeDatabase] = "distinct";

		  databaseSpecificEscapeChar[mysqlLikeDatabase] = "'\\\\'";

		  databaseSpecificBitAnd1[mysqlLikeDatabase] = "";
		  databaseSpecificBitAnd2[mysqlLikeDatabase] = " & ";
		  databaseSpecificBitAnd3[mysqlLikeDatabase] = "";
		  databaseSpecificDatepart1[mysqlLikeDatabase] = "";
		  databaseSpecificDatepart2[mysqlLikeDatabase] = "(";
		  databaseSpecificDatepart3[mysqlLikeDatabase] = ")";

		  databaseSpecificDummyTable[mysqlLikeDatabase] = "";
		  databaseSpecificTrueConstant[mysqlLikeDatabase] = "1";
		  databaseSpecificFalseConstant[mysqlLikeDatabase] = "0";
		  databaseSpecificIfNull[mysqlLikeDatabase] = "IFNULL";

		  databaseSpecificDaysComparator[mysqlLikeDatabase] = "DATEDIFF(#{currentTimestamp}, ${date}) >= ${days}";

		  databaseSpecificCollationForCaseSensitivity[mysqlLikeDatabase] = "";

		  addDatabaseSpecificStatement(mysqlLikeDatabase, "toggleForeignKey", "toggleForeignKey_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "selectProcessDefinitionsByQueryCriteria", "selectProcessDefinitionsByQueryCriteria_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "selectProcessDefinitionCountByQueryCriteria", "selectProcessDefinitionCountByQueryCriteria_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "selectDeploymentsByQueryCriteria", "selectDeploymentsByQueryCriteria_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "selectDeploymentCountByQueryCriteria", "selectDeploymentCountByQueryCriteria_mysql");

		  // related to CAM-8064
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "deleteExceptionByteArraysByIds", "deleteExceptionByteArraysByIds_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "deleteErrorDetailsByteArraysByIds", "deleteErrorDetailsByteArraysByIds_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "deleteHistoricDetailsByIds", "deleteHistoricDetailsByIds_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "deleteHistoricDetailByteArraysByIds", "deleteHistoricDetailByteArraysByIds_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "deleteHistoricIdentityLinksByTaskProcessInstanceIds", "deleteHistoricIdentityLinksByTaskProcessInstanceIds_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "deleteHistoricIdentityLinksByTaskCaseInstanceIds", "deleteHistoricIdentityLinksByTaskCaseInstanceIds_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "deleteHistoricDecisionInputInstanceByteArraysByDecisionInstanceIds", "deleteHistoricDecisionInputInstanceByteArraysByDecisionInstanceIds_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "deleteHistoricDecisionOutputInstanceByteArraysByDecisionInstanceIds", "deleteHistoricDecisionOutputInstanceByteArraysByDecisionInstanceIds_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "deleteHistoricVariableInstanceByIds", "deleteHistoricVariableInstanceByIds_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "deleteHistoricVariableInstanceByteArraysByIds", "deleteHistoricVariableInstanceByteArraysByIds_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "deleteCommentsByIds", "deleteCommentsByIds_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "deleteAttachmentByteArraysByIds", "deleteAttachmentByteArraysByIds_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "deleteAttachmentByIds", "deleteAttachmentByIds_mysql");

		  addDatabaseSpecificStatement(mysqlLikeDatabase, "deleteHistoricIncidentsByBatchIds", "deleteHistoricIncidentsByBatchIds_mysql");

		  // related to CAM-9505
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "updateUserOperationLogByRootProcessInstanceId", "updateUserOperationLogByRootProcessInstanceId_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "updateExternalTaskLogByRootProcessInstanceId", "updateExternalTaskLogByRootProcessInstanceId_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "updateHistoricIncidentsByRootProcessInstanceId", "updateHistoricIncidentsByRootProcessInstanceId_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "updateHistoricIncidentsByBatchId", "updateHistoricIncidentsByBatchId_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "updateIdentityLinkLogByRootProcessInstanceId", "updateIdentityLinkLogByRootProcessInstanceId_mysql");

		  // related to CAM-10172
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "updateUserOperationLogByProcessInstanceId", "updateUserOperationLogByProcessInstanceId_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "updateExternalTaskLogByProcessInstanceId", "updateExternalTaskLogByProcessInstanceId_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "updateHistoricIncidentsByProcessInstanceId", "updateHistoricIncidentsByProcessInstanceId_mysql");
		  addDatabaseSpecificStatement(mysqlLikeDatabase, "updateIdentityLinkLogByProcessInstanceId", "updateIdentityLinkLogByProcessInstanceId_mysql");

		  constants = new Dictionary<string, string>();
		  constants["constant.event"] = "'event'";
		  constants["constant.op_message"] = "CONCAT(NEW_VALUE_, '_|_', PROPERTY_)";
		  constants["constant_for_update"] = "for update";
		  constants["constant.datepart.quarter"] = "QUARTER";
		  constants["constant.datepart.month"] = "MONTH";
		  constants["constant.datepart.minute"] = "MINUTE";
		  constants["constant.null.startTime"] = "null START_TIME_";
		  constants["constant.varchar.cast"] = "'${key}'";
		  constants["constant.null.reporter"] = "NULL AS REPORTER_";
		  dbSpecificConstants[mysqlLikeDatabase] = constants;
		}

		// postgres specific
		databaseSpecificLimitBeforeStatements[POSTGRES] = "";
		optimizeDatabaseSpecificLimitBeforeWithoutOffsetStatements[POSTGRES] = "";
		databaseSpecificLimitAfterStatements[POSTGRES] = "LIMIT #{maxResults} OFFSET #{firstResult}";
		optimizeDatabaseSpecificLimitAfterWithoutOffsetStatements[POSTGRES] = "LIMIT #{maxResults}";
		databaseSpecificLimitBeforeWithoutOffsetStatements[POSTGRES] = "";
		databaseSpecificLimitAfterWithoutOffsetStatements[POSTGRES] = "LIMIT #{maxResults}";
		databaseSpecificInnerLimitAfterStatements[POSTGRES] = databaseSpecificLimitAfterStatements[POSTGRES];
		databaseSpecificLimitBetweenStatements[POSTGRES] = "";
		databaseSpecificLimitBetweenFilterStatements[POSTGRES] = "";
		databaseSpecificOrderByStatements[POSTGRES] = defaultOrderBy;
		databaseSpecificLimitBeforeNativeQueryStatements[POSTGRES] = "";
		databaseSpecificDistinct[POSTGRES] = "distinct";

		databaseSpecificEscapeChar[POSTGRES] = defaultEscapeChar;

		databaseSpecificBitAnd1[POSTGRES] = "";
		databaseSpecificBitAnd2[POSTGRES] = " & ";
		databaseSpecificBitAnd3[POSTGRES] = "";
		databaseSpecificDatepart1[POSTGRES] = "extract(";
		databaseSpecificDatepart2[POSTGRES] = " from ";
		databaseSpecificDatepart3[POSTGRES] = ")";

		databaseSpecificDummyTable[POSTGRES] = "";
		databaseSpecificTrueConstant[POSTGRES] = "true";
		databaseSpecificFalseConstant[POSTGRES] = "false";
		databaseSpecificIfNull[POSTGRES] = "COALESCE";

		databaseSpecificDaysComparator[POSTGRES] = "EXTRACT (DAY FROM #{currentTimestamp} - ${date}) >= ${days}";

		databaseSpecificCollationForCaseSensitivity[POSTGRES] = "";

		addDatabaseSpecificStatement(POSTGRES, "insertByteArray", "insertByteArray_postgres");
		addDatabaseSpecificStatement(POSTGRES, "updateByteArray", "updateByteArray_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectByteArray", "selectByteArray_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectByteArrays", "selectByteArrays_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectResourceByDeploymentIdAndResourceName", "selectResourceByDeploymentIdAndResourceName_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectResourceByDeploymentIdAndResourceNames", "selectResourceByDeploymentIdAndResourceNames_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectResourceByDeploymentIdAndResourceId", "selectResourceByDeploymentIdAndResourceId_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectResourceByDeploymentIdAndResourceIds", "selectResourceByDeploymentIdAndResourceIds_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectResourcesByDeploymentId", "selectResourcesByDeploymentId_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectLatestResourcesByDeploymentName", "selectLatestResourcesByDeploymentName_postgres");
		addDatabaseSpecificStatement(POSTGRES, "insertIdentityInfo", "insertIdentityInfo_postgres");
		addDatabaseSpecificStatement(POSTGRES, "updateIdentityInfo", "updateIdentityInfo_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectIdentityInfoById", "selectIdentityInfoById_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectIdentityInfoByUserIdAndKey", "selectIdentityInfoByUserIdAndKey_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectIdentityInfoByUserId", "selectIdentityInfoByUserId_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectIdentityInfoDetails", "selectIdentityInfoDetails_postgres");
		addDatabaseSpecificStatement(POSTGRES, "insertComment", "insertComment_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectCommentsByTaskId", "selectCommentsByTaskId_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectCommentsByProcessInstanceId", "selectCommentsByProcessInstanceId_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectCommentByTaskIdAndCommentId", "selectCommentByTaskIdAndCommentId_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectEventsByTaskId", "selectEventsByTaskId_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectFilterByQueryCriteria", "selectFilterByQueryCriteria_postgres");
		addDatabaseSpecificStatement(POSTGRES, "selectFilter", "selectFilter_postgres");

		addDatabaseSpecificStatement(POSTGRES, "deleteAttachmentsByRemovalTime", "deleteAttachmentsByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(POSTGRES, "deleteCommentsByRemovalTime", "deleteCommentsByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(POSTGRES, "deleteHistoricActivityInstancesByRemovalTime", "deleteHistoricActivityInstancesByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(POSTGRES, "deleteHistoricDecisionInputInstancesByRemovalTime", "deleteHistoricDecisionInputInstancesByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(POSTGRES, "deleteHistoricDecisionInstancesByRemovalTime", "deleteHistoricDecisionInstancesByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(POSTGRES, "deleteHistoricDecisionOutputInstancesByRemovalTime", "deleteHistoricDecisionOutputInstancesByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(POSTGRES, "deleteHistoricDetailsByRemovalTime", "deleteHistoricDetailsByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(POSTGRES, "deleteExternalTaskLogByRemovalTime", "deleteExternalTaskLogByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(POSTGRES, "deleteHistoricIdentityLinkLogByRemovalTime", "deleteHistoricIdentityLinkLogByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(POSTGRES, "deleteHistoricIncidentsByRemovalTime", "deleteHistoricIncidentsByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(POSTGRES, "deleteJobLogByRemovalTime", "deleteJobLogByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(POSTGRES, "deleteHistoricProcessInstancesByRemovalTime", "deleteHistoricProcessInstancesByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(POSTGRES, "deleteHistoricTaskInstancesByRemovalTime", "deleteHistoricTaskInstancesByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(POSTGRES, "deleteHistoricVariableInstancesByRemovalTime", "deleteHistoricVariableInstancesByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(POSTGRES, "deleteUserOperationLogByRemovalTime", "deleteUserOperationLogByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(POSTGRES, "deleteByteArraysByRemovalTime", "deleteByteArraysByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(POSTGRES, "deleteHistoricBatchesByRemovalTime", "deleteHistoricBatchesByRemovalTime_postgres_or_db2");

		constants = new Dictionary<string, string>();
		constants["constant.event"] = "'event'";
		constants["constant.op_message"] = "NEW_VALUE_ || '_|_' || PROPERTY_";
		constants["constant_for_update"] = "for update";
		constants["constant.datepart.quarter"] = "QUARTER";
		constants["constant.datepart.month"] = "MONTH";
		constants["constant.datepart.minute"] = "MINUTE";
		constants["constant.null.startTime"] = "null START_TIME_";
		constants["constant.varchar.cast"] = "cast('${key}' as varchar(64))";
		constants["constant.null.reporter"] = "CAST(NULL AS VARCHAR) AS REPORTER_";
		dbSpecificConstants[POSTGRES] = constants;

		// oracle
		databaseSpecificLimitBeforeStatements[ORACLE] = "select * from ( select a.*, ROWNUM rnum from (";
		optimizeDatabaseSpecificLimitBeforeWithoutOffsetStatements[ORACLE] = "select * from ( select a.*, ROWNUM rnum from (";
		databaseSpecificLimitAfterStatements[ORACLE] = "  ) a where ROWNUM < #{lastRow}) where rnum  >= #{firstRow}";
		optimizeDatabaseSpecificLimitAfterWithoutOffsetStatements[ORACLE] = "  ) a where ROWNUM <= #{maxResults})";
		databaseSpecificLimitBeforeWithoutOffsetStatements[ORACLE] = "";
		databaseSpecificLimitAfterWithoutOffsetStatements[ORACLE] = "AND ROWNUM <= #{maxResults}";
		databaseSpecificInnerLimitAfterStatements[ORACLE] = databaseSpecificLimitAfterStatements[ORACLE];
		databaseSpecificLimitBetweenStatements[ORACLE] = "";
		databaseSpecificLimitBetweenFilterStatements[ORACLE] = "";
		databaseSpecificOrderByStatements[ORACLE] = defaultOrderBy;
		databaseSpecificLimitBeforeNativeQueryStatements[ORACLE] = "";
		databaseSpecificDistinct[ORACLE] = "distinct";

		databaseSpecificEscapeChar[ORACLE] = defaultEscapeChar;

		databaseSpecificDummyTable[ORACLE] = "FROM DUAL";
		databaseSpecificBitAnd1[ORACLE] = "BITAND(";
		databaseSpecificBitAnd2[ORACLE] = ",";
		databaseSpecificBitAnd3[ORACLE] = ")";
		databaseSpecificDatepart1[ORACLE] = "to_number(to_char(";
		databaseSpecificDatepart2[ORACLE] = ",";
		databaseSpecificDatepart3[ORACLE] = "))";

		databaseSpecificTrueConstant[ORACLE] = "1";
		databaseSpecificFalseConstant[ORACLE] = "0";
		databaseSpecificIfNull[ORACLE] = "NVL";

		databaseSpecificDaysComparator[ORACLE] = "${date} <= #{currentTimestamp} - ${days}";

		databaseSpecificCollationForCaseSensitivity[ORACLE] = "";

		addDatabaseSpecificStatement(ORACLE, "selectHistoricProcessInstanceDurationReport", "selectHistoricProcessInstanceDurationReport_oracle");
		addDatabaseSpecificStatement(ORACLE, "selectHistoricTaskInstanceDurationReport", "selectHistoricTaskInstanceDurationReport_oracle");
		addDatabaseSpecificStatement(ORACLE, "selectHistoricTaskInstanceCountByTaskNameReport", "selectHistoricTaskInstanceCountByTaskNameReport_oracle");
		addDatabaseSpecificStatement(ORACLE, "selectFilterByQueryCriteria", "selectFilterByQueryCriteria_oracleDb2");
		addDatabaseSpecificStatement(ORACLE, "selectHistoricProcessInstanceIdsForCleanup", "selectHistoricProcessInstanceIdsForCleanup_oracle");
		addDatabaseSpecificStatement(ORACLE, "selectHistoricDecisionInstanceIdsForCleanup", "selectHistoricDecisionInstanceIdsForCleanup_oracle");
		addDatabaseSpecificStatement(ORACLE, "selectHistoricCaseInstanceIdsForCleanup", "selectHistoricCaseInstanceIdsForCleanup_oracle");
		addDatabaseSpecificStatement(ORACLE, "selectHistoricBatchIdsForCleanup", "selectHistoricBatchIdsForCleanup_oracle");

		addDatabaseSpecificStatement(ORACLE, "deleteAttachmentsByRemovalTime", "deleteAttachmentsByRemovalTime_oracle");
		addDatabaseSpecificStatement(ORACLE, "deleteCommentsByRemovalTime", "deleteCommentsByRemovalTime_oracle");
		addDatabaseSpecificStatement(ORACLE, "deleteHistoricActivityInstancesByRemovalTime", "deleteHistoricActivityInstancesByRemovalTime_oracle");
		addDatabaseSpecificStatement(ORACLE, "deleteHistoricDecisionInputInstancesByRemovalTime", "deleteHistoricDecisionInputInstancesByRemovalTime_oracle");
		addDatabaseSpecificStatement(ORACLE, "deleteHistoricDecisionInstancesByRemovalTime", "deleteHistoricDecisionInstancesByRemovalTime_oracle");
		addDatabaseSpecificStatement(ORACLE, "deleteHistoricDecisionOutputInstancesByRemovalTime", "deleteHistoricDecisionOutputInstancesByRemovalTime_oracle");
		addDatabaseSpecificStatement(ORACLE, "deleteHistoricDetailsByRemovalTime", "deleteHistoricDetailsByRemovalTime_oracle");
		addDatabaseSpecificStatement(ORACLE, "deleteExternalTaskLogByRemovalTime", "deleteExternalTaskLogByRemovalTime_oracle");
		addDatabaseSpecificStatement(ORACLE, "deleteHistoricIdentityLinkLogByRemovalTime", "deleteHistoricIdentityLinkLogByRemovalTime_oracle");
		addDatabaseSpecificStatement(ORACLE, "deleteHistoricIncidentsByRemovalTime", "deleteHistoricIncidentsByRemovalTime_oracle");
		addDatabaseSpecificStatement(ORACLE, "deleteJobLogByRemovalTime", "deleteJobLogByRemovalTime_oracle");
		addDatabaseSpecificStatement(ORACLE, "deleteHistoricProcessInstancesByRemovalTime", "deleteHistoricProcessInstancesByRemovalTime_oracle");
		addDatabaseSpecificStatement(ORACLE, "deleteHistoricTaskInstancesByRemovalTime", "deleteHistoricTaskInstancesByRemovalTime_oracle");
		addDatabaseSpecificStatement(ORACLE, "deleteHistoricVariableInstancesByRemovalTime", "deleteHistoricVariableInstancesByRemovalTime_oracle");
		addDatabaseSpecificStatement(ORACLE, "deleteUserOperationLogByRemovalTime", "deleteUserOperationLogByRemovalTime_oracle");
		addDatabaseSpecificStatement(ORACLE, "deleteByteArraysByRemovalTime", "deleteByteArraysByRemovalTime_oracle");
		addDatabaseSpecificStatement(ORACLE, "deleteHistoricBatchesByRemovalTime", "deleteHistoricBatchesByRemovalTime_oracle");

		constants = new Dictionary<string, string>();
		constants["constant.event"] = "cast('event' as nvarchar2(255))";
		constants["constant.op_message"] = "NEW_VALUE_ || '_|_' || PROPERTY_";
		constants["constant_for_update"] = "for update";
		constants["constant.datepart.quarter"] = "'Q'";
		constants["constant.datepart.month"] = "'MM'";
		constants["constant.datepart.minute"] = "'MI'";
		constants["constant.null.startTime"] = "null START_TIME_";
		constants["constant.varchar.cast"] = "'${key}'";
		constants["constant.null.reporter"] = "NULL AS REPORTER_";
		dbSpecificConstants[ORACLE] = constants;

		// db2
		databaseSpecificLimitBeforeStatements[DB2] = "SELECT SUB.* FROM (";
		optimizeDatabaseSpecificLimitBeforeWithoutOffsetStatements[DB2] = "";
		databaseSpecificInnerLimitAfterStatements[DB2] = ")RES ) SUB WHERE SUB.rnk >= #{firstRow} AND SUB.rnk < #{lastRow}";
		databaseSpecificLimitAfterStatements[DB2] = databaseSpecificInnerLimitAfterStatements[DB2] + " ORDER BY SUB.rnk";
		optimizeDatabaseSpecificLimitAfterWithoutOffsetStatements[DB2] = "FETCH FIRST ${maxResults} ROWS ONLY";
		databaseSpecificLimitBetweenStatements[DB2] = ", row_number() over (ORDER BY ${internalOrderBy}) rnk FROM ( select distinct RES.* ";
		databaseSpecificLimitBetweenFilterStatements[DB2] = ", row_number() over (ORDER BY ${internalOrderBy}) rnk FROM ( select distinct RES.ID_, RES.REV_, RES.RESOURCE_TYPE_, RES.NAME_, RES.OWNER_ ";
		databaseSpecificLimitBeforeWithoutOffsetStatements[DB2] = "";
		databaseSpecificLimitAfterWithoutOffsetStatements[DB2] = "FETCH FIRST ${maxResults} ROWS ONLY";
		databaseSpecificOrderByStatements[DB2] = defaultOrderBy;
		databaseSpecificLimitBeforeNativeQueryStatements[DB2] = "SELECT SUB.* FROM ( select RES.* , row_number() over (ORDER BY ${internalOrderBy}) rnk FROM (";
		databaseSpecificDistinct[DB2] = "";

		databaseSpecificEscapeChar[DB2] = defaultEscapeChar;

		databaseSpecificBitAnd1[DB2] = "BITAND(";
		databaseSpecificBitAnd2[DB2] = ", CAST(";
		databaseSpecificBitAnd3[DB2] = " AS Integer))";
		databaseSpecificDatepart1[DB2] = "";
		databaseSpecificDatepart2[DB2] = "(";
		databaseSpecificDatepart3[DB2] = ")";

		databaseSpecificDummyTable[DB2] = "FROM SYSIBM.SYSDUMMY1";
		databaseSpecificTrueConstant[DB2] = "1";
		databaseSpecificFalseConstant[DB2] = "0";
		databaseSpecificIfNull[DB2] = "NVL";

		databaseSpecificDaysComparator[DB2] = "${date} + ${days} DAYS <= #{currentTimestamp}";

		databaseSpecificCollationForCaseSensitivity[DB2] = "";

		addDatabaseSpecificStatement(DB2, "selectMeterLogAggregatedByTimeInterval", "selectMeterLogAggregatedByTimeInterval_db2_or_mssql");
		addDatabaseSpecificStatement(DB2, "selectExecutionByNativeQuery", "selectExecutionByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(DB2, "selectHistoricActivityInstanceByNativeQuery", "selectHistoricActivityInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(DB2, "selectHistoricCaseActivityInstanceByNativeQuery", "selectHistoricCaseActivityInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(DB2, "selectHistoricProcessInstanceByNativeQuery", "selectHistoricProcessInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(DB2, "selectHistoricCaseInstanceByNativeQuery", "selectHistoricCaseInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(DB2, "selectHistoricTaskInstanceByNativeQuery", "selectHistoricTaskInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(DB2, "selectHistoricVariableInstanceByNativeQuery", "selectHistoricVariableInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(DB2, "selectTaskByNativeQuery", "selectTaskByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(DB2, "selectUserByNativeQuery", "selectUserByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(DB2, "selectHistoricDecisionInstancesByNativeQuery", "selectHistoricDecisionInstancesByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(DB2, "selectFilterByQueryCriteria", "selectFilterByQueryCriteria_oracleDb2");

		addDatabaseSpecificStatement(DB2, "deleteAttachmentsByRemovalTime", "deleteAttachmentsByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(DB2, "deleteCommentsByRemovalTime", "deleteCommentsByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(DB2, "deleteHistoricActivityInstancesByRemovalTime", "deleteHistoricActivityInstancesByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(DB2, "deleteHistoricDecisionInputInstancesByRemovalTime", "deleteHistoricDecisionInputInstancesByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(DB2, "deleteHistoricDecisionInstancesByRemovalTime", "deleteHistoricDecisionInstancesByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(DB2, "deleteHistoricDecisionOutputInstancesByRemovalTime", "deleteHistoricDecisionOutputInstancesByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(DB2, "deleteHistoricDetailsByRemovalTime", "deleteHistoricDetailsByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(DB2, "deleteExternalTaskLogByRemovalTime", "deleteExternalTaskLogByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(DB2, "deleteHistoricIdentityLinkLogByRemovalTime", "deleteHistoricIdentityLinkLogByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(DB2, "deleteHistoricIncidentsByRemovalTime", "deleteHistoricIncidentsByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(DB2, "deleteJobLogByRemovalTime", "deleteJobLogByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(DB2, "deleteHistoricProcessInstancesByRemovalTime", "deleteHistoricProcessInstancesByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(DB2, "deleteHistoricTaskInstancesByRemovalTime", "deleteHistoricTaskInstancesByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(DB2, "deleteHistoricVariableInstancesByRemovalTime", "deleteHistoricVariableInstancesByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(DB2, "deleteUserOperationLogByRemovalTime", "deleteUserOperationLogByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(DB2, "deleteByteArraysByRemovalTime", "deleteByteArraysByRemovalTime_postgres_or_db2");
		addDatabaseSpecificStatement(DB2, "deleteHistoricBatchesByRemovalTime", "deleteHistoricBatchesByRemovalTime_postgres_or_db2");

		constants = new Dictionary<string, string>();
		constants["constant.event"] = "'event'";
		constants["constant.op_message"] = "CAST(CONCAT(CONCAT(COALESCE(NEW_VALUE_,''), '_|_'), COALESCE(PROPERTY_,'')) as varchar(255))";
		constants["constant_for_update"] = "for read only with rs use and keep update locks";
		constants["constant.datepart.quarter"] = "QUARTER";
		constants["constant.datepart.month"] = "MONTH";
		constants["constant.datepart.minute"] = "MINUTE";
		constants["constant.null.startTime"] = "CAST(NULL as timestamp) as START_TIME_";
		constants["constant.varchar.cast"] = "cast('${key}' as varchar(64))";
		constants["constant.null.reporter"] = "CAST(NULL AS VARCHAR(255)) AS REPORTER_";
		dbSpecificConstants[DB2] = constants;

		// mssql
		databaseSpecificLimitBeforeStatements[MSSQL] = "SELECT SUB.* FROM (";
		optimizeDatabaseSpecificLimitBeforeWithoutOffsetStatements[MSSQL] = "";
		databaseSpecificInnerLimitAfterStatements[MSSQL] = ")RES ) SUB WHERE SUB.rnk >= #{firstRow} AND SUB.rnk < #{lastRow}";
		databaseSpecificLimitAfterStatements[MSSQL] = databaseSpecificInnerLimitAfterStatements[MSSQL] + " ORDER BY SUB.rnk";
		optimizeDatabaseSpecificLimitAfterWithoutOffsetStatements[MSSQL] = "";
		databaseSpecificLimitBetweenStatements[MSSQL] = ", row_number() over (ORDER BY ${internalOrderBy}) rnk FROM ( select distinct RES.* ";
		databaseSpecificLimitBetweenFilterStatements[MSSQL] = "";
		databaseSpecificLimitBeforeWithoutOffsetStatements[MSSQL] = "TOP (#{maxResults})";
		databaseSpecificLimitAfterWithoutOffsetStatements[MSSQL] = "";
		databaseSpecificOrderByStatements[MSSQL] = "";
		databaseSpecificLimitBeforeNativeQueryStatements[MSSQL] = "SELECT SUB.* FROM ( select RES.* , row_number() over (ORDER BY ${internalOrderBy}) rnk FROM (";
		databaseSpecificDistinct[MSSQL] = "";

		databaseSpecificEscapeChar[MSSQL] = defaultEscapeChar;

		databaseSpecificBitAnd1[MSSQL] = "";
		databaseSpecificBitAnd2[MSSQL] = " &";
		databaseSpecificBitAnd3[MSSQL] = "";
		databaseSpecificDatepart1[MSSQL] = "datepart(";
		databaseSpecificDatepart2[MSSQL] = ",";
		databaseSpecificDatepart3[MSSQL] = ")";

		databaseSpecificDummyTable[MSSQL] = "";
		databaseSpecificTrueConstant[MSSQL] = "1";
		databaseSpecificFalseConstant[MSSQL] = "0";
		databaseSpecificIfNull[MSSQL] = "ISNULL";

		databaseSpecificDaysComparator[MSSQL] = "DATEDIFF(DAY, ${date}, #{currentTimestamp}) >= ${days}";

		databaseSpecificCollationForCaseSensitivity[MSSQL] = "COLLATE Latin1_General_CS_AS";

		addDatabaseSpecificStatement(MSSQL, "selectMeterLogAggregatedByTimeInterval", "selectMeterLogAggregatedByTimeInterval_db2_or_mssql");
		addDatabaseSpecificStatement(MSSQL, "selectExecutionByNativeQuery", "selectExecutionByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(MSSQL, "selectHistoricActivityInstanceByNativeQuery", "selectHistoricActivityInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(MSSQL, "selectHistoricCaseActivityInstanceByNativeQuery", "selectHistoricCaseActivityInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(MSSQL, "selectHistoricProcessInstanceByNativeQuery", "selectHistoricProcessInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(MSSQL, "selectHistoricCaseInstanceByNativeQuery", "selectHistoricCaseInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(MSSQL, "selectHistoricTaskInstanceByNativeQuery", "selectHistoricTaskInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(MSSQL, "selectHistoricVariableInstanceByNativeQuery", "selectHistoricVariableInstanceByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(MSSQL, "selectTaskByNativeQuery", "selectTaskByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(MSSQL, "selectUserByNativeQuery", "selectUserByNativeQuery_mssql_or_db2");
		addDatabaseSpecificStatement(MSSQL, "lockDeploymentLockProperty", "lockDeploymentLockProperty_mssql");
		addDatabaseSpecificStatement(MSSQL, "lockHistoryCleanupJobLockProperty", "lockHistoryCleanupJobLockProperty_mssql");
		addDatabaseSpecificStatement(MSSQL, "lockStartupLockProperty", "lockStartupLockProperty_mssql");
		addDatabaseSpecificStatement(MSSQL, "selectEventSubscriptionsByNameAndExecution", "selectEventSubscriptionsByNameAndExecution_mssql");
		addDatabaseSpecificStatement(MSSQL, "selectEventSubscriptionsByExecutionAndType", "selectEventSubscriptionsByExecutionAndType_mssql");
		addDatabaseSpecificStatement(MSSQL, "selectHistoricDecisionInstancesByNativeQuery", "selectHistoricDecisionInstancesByNativeQuery_mssql_or_db2");

		constants = new Dictionary<string, string>();
		constants["constant.event"] = "'event'";
		constants["constant.op_message"] = "NEW_VALUE_ + '_|_' + PROPERTY_";
		constants["constant.datepart.quarter"] = "QUARTER";
		constants["constant.datepart.month"] = "MONTH";
		constants["constant.datepart.minute"] = "MINUTE";
		constants["constant.null.startTime"] = "CAST(NULL AS datetime2) AS START_TIME_";
		constants["constant.varchar.cast"] = "'${key}'";
		constants["constant.null.reporter"] = "NULL AS REPORTER_";
		dbSpecificConstants[MSSQL] = constants;
	  }

	  protected internal string databaseType;
	  protected internal string databaseTablePrefix = "";
	  /// <summary>
	  /// In some situations you want to set the schema to use for table checks /
	  /// generation if the database metadata doesn't return that correctly, see
	  /// https://jira.codehaus.org/browse/ACT-1220,
	  /// https://jira.codehaus.org/browse/ACT-1062
	  /// </summary>
	  protected internal string databaseSchema;
	  protected internal SqlSessionFactory sqlSessionFactory;
	  protected internal IdGenerator idGenerator;
	  protected internal IDictionary<string, string> statementMappings;
	  protected internal IDictionary<Type, string> insertStatements = new ConcurrentDictionary<Type, string>();
	  protected internal IDictionary<Type, string> updateStatements = new ConcurrentDictionary<Type, string>();
	  protected internal IDictionary<Type, string> deleteStatements = new ConcurrentDictionary<Type, string>();
	  protected internal IDictionary<Type, string> selectStatements = new ConcurrentDictionary<Type, string>();
	  protected internal bool isDbIdentityUsed = true;
	  protected internal bool isDbHistoryUsed = true;
	  protected internal bool cmmnEnabled = true;
	  protected internal bool dmnEnabled = true;

	  public virtual Type SessionType
	  {
		  get
		  {
			return typeof(DbSqlSession);
		  }
	  }

	  public virtual Session openSession()
	  {
		return new DbSqlSession(this);
	  }

	  // insert, update and delete statements /////////////////////////////////////

	  public virtual string getInsertStatement(DbEntity @object)
	  {
		return getStatement(@object.GetType(), insertStatements, "insert");
	  }

	  public virtual string getUpdateStatement(DbEntity @object)
	  {
		return getStatement(@object.GetType(), updateStatements, "update");
	  }

	  public virtual string getDeleteStatement(Type persistentObjectClass)
	  {
		return getStatement(persistentObjectClass, deleteStatements, "delete");
	  }

	  public virtual string getSelectStatement(Type persistentObjectClass)
	  {
		return getStatement(persistentObjectClass, selectStatements, "select");
	  }

	  private string getStatement(Type persistentObjectClass, IDictionary<Type, string> cachedStatements, string prefix)
	  {
		string statement = cachedStatements[persistentObjectClass];
		if (!string.ReferenceEquals(statement, null))
		{
		  return statement;
		}
		statement = prefix + ClassNameUtil.getClassNameWithoutPackage(persistentObjectClass);
		statement = statement.Substring(0, statement.Length - 6); // "Entity".length() = 6
		cachedStatements[persistentObjectClass] = statement;
		return statement;
	  }

	  // db specific mappings /////////////////////////////////////////////////////

	  protected internal static void addDatabaseSpecificStatement(string databaseType, string activitiStatement, string ibatisStatement)
	  {
		IDictionary<string, string> specificStatements = databaseSpecificStatements[databaseType];
		if (specificStatements == null)
		{
		  specificStatements = new Dictionary<string, string>();
		  databaseSpecificStatements[databaseType] = specificStatements;
		}
		specificStatements[activitiStatement] = ibatisStatement;
	  }

	  public virtual string mapStatement(string statement)
	  {
		if (statementMappings == null)
		{
		  return statement;
		}
		string mappedStatement = statementMappings[statement];
		return (!string.ReferenceEquals(mappedStatement, null) ? mappedStatement : statement);
	  }

	  // customized getters and setters ///////////////////////////////////////////

	  public virtual string DatabaseType
	  {
		  set
		  {
			this.databaseType = value;
			this.statementMappings = databaseSpecificStatements[value];
		  }
		  get
		  {
			return databaseType;
		  }
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual SqlSessionFactory SqlSessionFactory
	  {
		  get
		  {
			return sqlSessionFactory;
		  }
		  set
		  {
			this.sqlSessionFactory = value;
		  }
	  }


	  public virtual IdGenerator IdGenerator
	  {
		  get
		  {
			return idGenerator;
		  }
		  set
		  {
			this.idGenerator = value;
		  }
	  }





	  public virtual IDictionary<string, string> StatementMappings
	  {
		  get
		  {
			return statementMappings;
		  }
		  set
		  {
			this.statementMappings = value;
		  }
	  }




	  public virtual IDictionary<Type, string> InsertStatements
	  {
		  get
		  {
			return insertStatements;
		  }
		  set
		  {
			this.insertStatements = value;
		  }
	  }




	  public virtual IDictionary<Type, string> UpdateStatements
	  {
		  get
		  {
			return updateStatements;
		  }
		  set
		  {
			this.updateStatements = value;
		  }
	  }




	  public virtual IDictionary<Type, string> DeleteStatements
	  {
		  get
		  {
			return deleteStatements;
		  }
		  set
		  {
			this.deleteStatements = value;
		  }
	  }




	  public virtual IDictionary<Type, string> SelectStatements
	  {
		  get
		  {
			return selectStatements;
		  }
		  set
		  {
			this.selectStatements = value;
		  }
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


	  public virtual string DatabaseTablePrefix
	  {
		  set
		  {
			this.databaseTablePrefix = value;
		  }
		  get
		  {
			return databaseTablePrefix;
		  }
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



	}

}