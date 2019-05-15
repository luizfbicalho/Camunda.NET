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
namespace org.camunda.bpm.engine.impl.db.sql
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using BatchResult = org.apache.ibatis.executor.BatchResult;
	using MappedStatement = org.apache.ibatis.mapping.MappedStatement;
	using SqlSession = org.apache.ibatis.session.SqlSession;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbBulkOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbBulkOperation;
	using DbEntityOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbEntityOperation;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;


	/// 
	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class DbSqlSession : AbstractPersistenceSession
	{

	  protected internal new static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  protected internal SqlSession sqlSession;
	  protected internal DbSqlSessionFactory dbSqlSessionFactory;

	  protected internal string connectionMetadataDefaultCatalog = null;
	  protected internal string connectionMetadataDefaultSchema = null;

	  public DbSqlSession(DbSqlSessionFactory dbSqlSessionFactory)
	  {
		this.dbSqlSessionFactory = dbSqlSessionFactory;
		this.sqlSession = dbSqlSessionFactory.SqlSessionFactory.openSession();
	  }

	  public DbSqlSession(DbSqlSessionFactory dbSqlSessionFactory, Connection connection, string catalog, string schema)
	  {
		this.dbSqlSessionFactory = dbSqlSessionFactory;
		this.sqlSession = dbSqlSessionFactory.SqlSessionFactory.openSession(connection);
		this.connectionMetadataDefaultCatalog = catalog;
		this.connectionMetadataDefaultSchema = schema;
	  }

	  public override IList<BatchResult> flushOperations()
	  {
		return sqlSession.flushStatements();
	  }

	  // select ////////////////////////////////////////////

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public java.util.List<?> selectList(String statement, Object parameter)
	  public override IList<object> selectList(string statement, object parameter)
	  {
		statement = dbSqlSessionFactory.mapStatement(statement);
		IList<object> resultList = sqlSession.selectList(statement, parameter);
		foreach (object @object in resultList)
		{
		  fireEntityLoaded(@object);
		}
		return resultList;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.engine.impl.db.DbEntity> T selectById(Class<T> type, String id)
	  public override T selectById<T>(Type<T> type, string id) where T : org.camunda.bpm.engine.impl.db.DbEntity
	  {
		string selectStatement = dbSqlSessionFactory.getSelectStatement(type);
		selectStatement = dbSqlSessionFactory.mapStatement(selectStatement);
		ensureNotNull("no select statement for " + type + " in the ibatis mapping files", "selectStatement", selectStatement);

		object result = sqlSession.selectOne(selectStatement, id);
		fireEntityLoaded(result);
		return (T) result;
	  }

	  public override object selectOne(string statement, object parameter)
	  {
		statement = dbSqlSessionFactory.mapStatement(statement);
		object result = sqlSession.selectOne(statement, parameter);
		fireEntityLoaded(result);
		return result;
	  }

	  // lock ////////////////////////////////////////////

	  public override void @lock(string statement, object parameter)
	  {
		// do not perform locking if H2 database is used. H2 uses table level locks
		// by default which may cause deadlocks if the deploy command needs to get a new
		// Id using the DbIdGenerator while performing a deployment.
		if (!DbSqlSessionFactory.H2.Equals(dbSqlSessionFactory.DatabaseType))
		{
		  string mappedStatement = dbSqlSessionFactory.mapStatement(statement);
		  if (!Context.ProcessEngineConfiguration.JdbcBatchProcessing)
		  {
			sqlSession.update(mappedStatement, parameter);
		  }
		  else
		  {
			sqlSession.selectList(mappedStatement, parameter);
		  }
		}
	  }

	  // insert //////////////////////////////////////////

	  protected internal override void insertEntity(DbEntityOperation operation)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.db.DbEntity dbEntity = operation.getEntity();
		DbEntity dbEntity = operation.Entity;

		// get statement
		string insertStatement = dbSqlSessionFactory.getInsertStatement(dbEntity);
		insertStatement = dbSqlSessionFactory.mapStatement(insertStatement);
		ensureNotNull("no insert statement for " + dbEntity.GetType() + " in the ibatis mapping files", "insertStatement", insertStatement);

		// execute the insert
		executeInsertEntity(insertStatement, dbEntity);

		// perform post insert actions on entity
		entityInserted(dbEntity);
	  }

	  protected internal virtual void executeInsertEntity(string insertStatement, object parameter)
	  {
		LOG.executeDatabaseOperation("INSERT", parameter);
		sqlSession.insert(insertStatement, parameter);

		// set revision of our copy to 1
		if (parameter is HasDbRevision)
		{
		  HasDbRevision versionedObject = (HasDbRevision) parameter;
		  versionedObject.Revision = 1;
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void entityInserted(final org.camunda.bpm.engine.impl.db.DbEntity entity)
	  protected internal virtual void entityInserted(DbEntity entity)
	  {
		// nothing to do
	  }

	  // delete ///////////////////////////////////////////

	  protected internal override void deleteEntity(DbEntityOperation operation)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.db.DbEntity dbEntity = operation.getEntity();
		DbEntity dbEntity = operation.Entity;

		// get statement
		string deleteStatement = dbSqlSessionFactory.getDeleteStatement(dbEntity.GetType());
		ensureNotNull("no delete statement for " + dbEntity.GetType() + " in the ibatis mapping files", "deleteStatement", deleteStatement);

		LOG.executeDatabaseOperation("DELETE", dbEntity);

		// execute the delete
		int nrOfRowsDeleted = executeDelete(deleteStatement, dbEntity);
		operation.RowsAffected = nrOfRowsDeleted;

		// It only makes sense to check for optimistic locking exceptions for objects that actually have a revision
		if (dbEntity is HasDbRevision && nrOfRowsDeleted == 0)
		{
		  operation.Failed = true;
		  return;
		}

		// perform post delete action
		entityDeleted(dbEntity);
	  }

	  protected internal virtual int executeDelete(string deleteStatement, object parameter)
	  {
		// map the statement
		deleteStatement = dbSqlSessionFactory.mapStatement(deleteStatement);
		return sqlSession.delete(deleteStatement, parameter);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void entityDeleted(final org.camunda.bpm.engine.impl.db.DbEntity entity)
	  protected internal virtual void entityDeleted(DbEntity entity)
	  {
		// nothing to do
	  }

	  protected internal override void deleteBulk(DbBulkOperation operation)
	  {
		string statement = operation.Statement;
		object parameter = operation.Parameter;

		LOG.executeDatabaseBulkOperation("DELETE", statement, parameter);

		int rowsAffected = executeDelete(statement, parameter);
		operation.RowsAffected = rowsAffected;
	  }

	  // update ////////////////////////////////////////

	  protected internal override void updateEntity(DbEntityOperation operation)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.db.DbEntity dbEntity = operation.getEntity();
		DbEntity dbEntity = operation.Entity;

		string updateStatement = dbSqlSessionFactory.getUpdateStatement(dbEntity);
		ensureNotNull("no update statement for " + dbEntity.GetType() + " in the ibatis mapping files", "updateStatement", updateStatement);

		LOG.executeDatabaseOperation("UPDATE", dbEntity);

		if (Context.ProcessEngineConfiguration.JdbcBatchProcessing)
		{
		  // execute update
		  executeUpdate(updateStatement, dbEntity);
		}
		else
		{
		  // execute update
		  int numOfRowsUpdated = executeUpdate(updateStatement, dbEntity);

		  if (dbEntity is HasDbRevision)
		  {
			if (numOfRowsUpdated != 1)
			{
			  // failed with optimistic locking
			  operation.Failed = true;
			  return;
			}
			else
			{
			  // increment revision of our copy
			  HasDbRevision versionedObject = (HasDbRevision) dbEntity;
			  versionedObject.Revision = versionedObject.RevisionNext;
			}
		  }
		}

		// perform post update action
		entityUpdated(dbEntity);
	  }

	  public override int executeUpdate(string updateStatement, object parameter)
	  {
		updateStatement = dbSqlSessionFactory.mapStatement(updateStatement);
		return sqlSession.update(updateStatement, parameter);
	  }

	  public override int executeNonEmptyUpdateStmt(string updateStmt, object parameter)
	  {
		updateStmt = dbSqlSessionFactory.mapStatement(updateStmt);

		//if mapped statement is empty, which can happens for some databases, we have no need to execute it
		MappedStatement mappedStatement = sqlSession.Configuration.getMappedStatement(updateStmt);
		if (mappedStatement.getBoundSql(parameter).Sql.Empty)
		{
		  return 0;
		}

		return sqlSession.update(updateStmt, parameter);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void entityUpdated(final org.camunda.bpm.engine.impl.db.DbEntity entity)
	  protected internal virtual void entityUpdated(DbEntity entity)
	  {
		// nothing to do
	  }

	  protected internal override void updateBulk(DbBulkOperation operation)
	  {
		string statement = operation.Statement;
		object parameter = operation.Parameter;

		LOG.executeDatabaseBulkOperation("UPDATE", statement, parameter);

		executeUpdate(statement, parameter);
	  }

	  // flush ////////////////////////////////////////////////////////////////////

	  public override void flush()
	  {
		// nothing to do
	  }

	  public override void close()
	  {
		sqlSession.close();
	  }

	  public override void commit()
	  {
		sqlSession.commit();
	  }

	  public override void rollback()
	  {
		sqlSession.rollback();
	  }

	  // schema operations ////////////////////////////////////////////////////////

	  public override void dbSchemaCheckVersion()
	  {
		try
		{
		  string dbVersion = DbVersion;
		  if (!org.camunda.bpm.engine.ProcessEngine_Fields.VERSION.Equals(dbVersion))
		  {
			throw LOG.wrongDbVersionException(org.camunda.bpm.engine.ProcessEngine_Fields.VERSION, dbVersion);
		  }

		  IList<string> missingComponents = new List<string>();
		  if (!EngineTablePresent)
		  {
			missingComponents.Add("engine");
		  }
		  if (dbSqlSessionFactory.DbHistoryUsed && !HistoryTablePresent)
		  {
			missingComponents.Add("history");
		  }
		  if (dbSqlSessionFactory.DbIdentityUsed && !IdentityTablePresent)
		  {
			missingComponents.Add("identity");
		  }
		  if (dbSqlSessionFactory.CmmnEnabled && !CmmnTablePresent)
		  {
			missingComponents.Add("case.engine");
		  }
		  if (dbSqlSessionFactory.DmnEnabled && !DmnTablePresent)
		  {
			missingComponents.Add("decision.engine");
		  }

		  if (missingComponents.Count > 0)
		  {
			throw LOG.missingTableException(missingComponents);
		  }

		}
		catch (Exception e)
		{
		  if (isMissingTablesException(e))
		  {
			throw LOG.missingActivitiTablesException();
		  }
		  else
		  {
			if (e is Exception)
			{
			  throw (Exception) e;
			}
			else
			{
			  throw LOG.unableToFetchDbSchemaVersion(e);
			}
		  }
		}
	  }

	  protected internal override string DbVersion
	  {
		  get
		  {
			string selectSchemaVersionStatement = dbSqlSessionFactory.mapStatement("selectDbSchemaVersion");
			return (string) sqlSession.selectOne(selectSchemaVersionStatement);
		  }
	  }

	  protected internal override void dbSchemaCreateIdentity()
	  {
		executeMandatorySchemaResource("create", "identity");
	  }

	  protected internal override void dbSchemaCreateHistory()
	  {
		executeMandatorySchemaResource("create", "history");
	  }

	  protected internal override void dbSchemaCreateEngine()
	  {
		executeMandatorySchemaResource("create", "engine");
	  }

	  protected internal override void dbSchemaCreateCmmn()
	  {
		executeMandatorySchemaResource("create", "case.engine");
	  }

	  protected internal override void dbSchemaCreateCmmnHistory()
	  {
		executeMandatorySchemaResource("create", "case.history");
	  }

	  protected internal override void dbSchemaCreateDmn()
	  {
		executeMandatorySchemaResource("create", "decision.engine");
	  }


	  protected internal override void dbSchemaCreateDmnHistory()
	  {
		executeMandatorySchemaResource("create", "decision.history");
	  }

	  protected internal override void dbSchemaDropIdentity()
	  {
		executeMandatorySchemaResource("drop", "identity");
	  }

	  protected internal override void dbSchemaDropHistory()
	  {
		executeMandatorySchemaResource("drop", "history");
	  }

	  protected internal override void dbSchemaDropEngine()
	  {
		executeMandatorySchemaResource("drop", "engine");
	  }

	  protected internal override void dbSchemaDropCmmn()
	  {
		executeMandatorySchemaResource("drop", "case.engine");
	  }

	  protected internal override void dbSchemaDropCmmnHistory()
	  {
		executeMandatorySchemaResource("drop", "case.history");
	  }

	  protected internal override void dbSchemaDropDmn()
	  {
		executeMandatorySchemaResource("drop", "decision.engine");
	  }

	  protected internal override void dbSchemaDropDmnHistory()
	  {
		executeMandatorySchemaResource("drop", "decision.history");
	  }

	  public virtual void executeMandatorySchemaResource(string operation, string component)
	  {
		executeSchemaResource(operation, component, getResourceForDbOperation(operation, operation, component), false);
	  }

	  public static string[] JDBC_METADATA_TABLE_TYPES = new string[] {"TABLE"};

	  public override bool EngineTablePresent
	  {
		  get
		  {
			return isTablePresent("ACT_RU_EXECUTION");
		  }
	  }
	  public override bool HistoryTablePresent
	  {
		  get
		  {
			return isTablePresent("ACT_HI_PROCINST");
		  }
	  }
	  public override bool IdentityTablePresent
	  {
		  get
		  {
			return isTablePresent("ACT_ID_USER");
		  }
	  }

	  public override bool CmmnTablePresent
	  {
		  get
		  {
			return isTablePresent("ACT_RE_CASE_DEF");
		  }
	  }

	  public override bool CmmnHistoryTablePresent
	  {
		  get
		  {
			return isTablePresent("ACT_HI_CASEINST");
		  }
	  }

	  public override bool DmnTablePresent
	  {
		  get
		  {
			return isTablePresent("ACT_RE_DECISION_DEF");
		  }
	  }

	  public override bool DmnHistoryTablePresent
	  {
		  get
		  {
			return isTablePresent("ACT_HI_DECINST");
		  }
	  }

	  public virtual bool isTablePresent(string tableName)
	  {
		tableName = prependDatabaseTablePrefix(tableName);
		Connection connection = null;
		try
		{
		  connection = sqlSession.Connection;
		  DatabaseMetaData databaseMetaData = connection.MetaData;
		  ResultSet tables = null;

		  string schema = this.connectionMetadataDefaultSchema;
		  if (!string.ReferenceEquals(dbSqlSessionFactory.DatabaseSchema, null))
		  {
			schema = dbSqlSessionFactory.DatabaseSchema;
		  }

		  string databaseType = dbSqlSessionFactory.DatabaseType;

		  if (DbSqlSessionFactory.POSTGRES.Equals(databaseType))
		  {
			tableName = tableName.ToLower();
		  }

		  try
		  {
			tables = databaseMetaData.getTables(this.connectionMetadataDefaultCatalog, schema, tableName, JDBC_METADATA_TABLE_TYPES);
			return tables.next();
		  }
		  finally
		  {
			if (tables != null)
			{
			  tables.close();
			}
		  }

		}
		catch (Exception e)
		{
		  throw LOG.checkDatabaseTableException(e);
		}
	  }

	  public override IList<string> TableNamesPresent
	  {
		  get
		  {
			IList<string> tableNames = new List<string>();
    
			try
			{
			  ResultSet tablesRs = null;
    
			  try
			  {
				if (DbSqlSessionFactory.ORACLE.Equals(DbSqlSessionFactory.DatabaseType))
				{
				  tableNames = TablesPresentInOracleDatabase;
				}
				else
				{
				  Connection connection = SqlSession.Connection;
    
				  string databaseTablePrefix = DbSqlSessionFactory.DatabaseTablePrefix;
				  string schema = DbSqlSessionFactory.DatabaseSchema;
				  string tableNameFilter = prependDatabaseTablePrefix("ACT_%");
    
				  // for postgres we have to use lower case
				  if (DbSqlSessionFactory.POSTGRES.Equals(DbSqlSessionFactory.DatabaseType))
				  {
					schema = string.ReferenceEquals(schema, null) ? schema : schema.ToLower();
					tableNameFilter = tableNameFilter.ToLower();
				  }
    
				  DatabaseMetaData databaseMetaData = connection.MetaData;
				  tablesRs = databaseMetaData.getTables(null, schema, tableNameFilter, DbSqlSession.JDBC_METADATA_TABLE_TYPES);
				  while (tablesRs.next())
				  {
					string tableName = tablesRs.getString("TABLE_NAME");
					if (databaseTablePrefix.Length > 0)
					{
					  tableName = databaseTablePrefix + tableName;
					}
					tableName = tableName.ToUpper();
					tableNames.Add(tableName);
				  }
				  LOG.fetchDatabaseTables("jdbc metadata", tableNames);
				}
			  }
			  catch (SQLException se)
			  {
				throw se;
			  }
			  finally
			  {
				if (tablesRs != null)
				{
				  tablesRs.close();
				}
			  }
			}
			catch (Exception e)
			{
			  throw LOG.getDatabaseTableNameException(e);
			}
    
			return tableNames;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.util.List<String> getTablesPresentInOracleDatabase() throws java.sql.SQLException
	  protected internal virtual IList<string> TablesPresentInOracleDatabase
	  {
		  get
		  {
			IList<string> tableNames = new List<string>();
			Connection connection = null;
			PreparedStatement prepStat = null;
			ResultSet tablesRs = null;
			string selectTableNamesFromOracle = "SELECT table_name FROM all_tables WHERE table_name LIKE ?";
			string databaseTablePrefix = DbSqlSessionFactory.DatabaseTablePrefix;
    
			try
			{
			  connection = Context.ProcessEngineConfiguration.DataSource.Connection;
			  prepStat = connection.prepareStatement(selectTableNamesFromOracle);
			  prepStat.setString(1, databaseTablePrefix + "ACT_%");
    
			  tablesRs = prepStat.executeQuery();
			  while (tablesRs.next())
			  {
				string tableName = tablesRs.getString("TABLE_NAME");
				tableName = tableName.ToUpper();
				tableNames.Add(tableName);
			  }
			  LOG.fetchDatabaseTables("oracle all_tables", tableNames);
    
			}
			finally
			{
			  if (tablesRs != null)
			  {
				tablesRs.close();
			  }
			  if (prepStat != null)
			  {
				prepStat.close();
			  }
			  if (connection != null)
			  {
				connection.close();
			  }
			}
    
			return tableNames;
		  }
	  }


	  public virtual string prependDatabaseTablePrefix(string tableName)
	  {
		string prefixWithoutSchema = dbSqlSessionFactory.DatabaseTablePrefix;
		string schema = dbSqlSessionFactory.DatabaseSchema;
		if (string.ReferenceEquals(prefixWithoutSchema, null))
		{
		  return tableName;
		}
		if (string.ReferenceEquals(schema, null))
		{
		  return prefixWithoutSchema + tableName;
		}

		if (prefixWithoutSchema.StartsWith(schema + ".", StringComparison.Ordinal))
		{
		  prefixWithoutSchema = prefixWithoutSchema.Substring(schema.Length + 1);
		}

		return prefixWithoutSchema + tableName;
	  }

	  public virtual string getResourceForDbOperation(string directory, string operation, string component)
	  {
		string databaseType = dbSqlSessionFactory.DatabaseType;
		return "org/camunda/bpm/engine/db/" + directory + "/activiti." + databaseType + "." + operation + "." + component + ".sql";
	  }

	  public virtual void executeSchemaResource(string operation, string component, string resourceName, bool isOptional)
	  {
		Stream inputStream = null;
		try
		{
		  inputStream = ReflectUtil.getResourceAsStream(resourceName);
		  if (inputStream == null)
		  {
			if (isOptional)
			{
			  LOG.missingSchemaResource(resourceName, operation);
			}
			else
			{
			  throw LOG.missingSchemaResourceException(resourceName, operation);
			}
		  }
		  else
		  {
			executeSchemaResource(operation, component, resourceName, inputStream);
		  }

		}
		finally
		{
		  IoUtil.closeSilently(inputStream);
		}
	  }

	  public virtual void executeSchemaResource(string schemaFileResourceName)
	  {
		FileStream inputStream = null;
		try
		{
		  inputStream = new FileStream(schemaFileResourceName, FileMode.Open, FileAccess.Read);
		  executeSchemaResource("schema operation", "process engine", schemaFileResourceName, inputStream);
		}
		catch (FileNotFoundException e)
		{
		  throw LOG.missingSchemaResourceFileException(schemaFileResourceName, e);
		}
		finally
		{
		  IoUtil.closeSilently(inputStream);
		}
	  }

	  private void executeSchemaResource(string operation, string component, string resourceName, Stream inputStream)
	  {
		string sqlStatement = null;
		string exceptionSqlStatement = null;
		try
		{
		  Connection connection = sqlSession.Connection;
		  Exception exception = null;
		  sbyte[] bytes = IoUtil.readInputStream(inputStream, resourceName);
		  string ddlStatements = StringHelper.NewString(bytes);
		  StreamReader reader = new StreamReader(new StringReader(ddlStatements));
		  string line = readNextTrimmedLine(reader);

		  IList<string> logLines = new List<string>();

		  while (!string.ReferenceEquals(line, null))
		  {
			if (line.StartsWith("# ", StringComparison.Ordinal))
			{
			  logLines.Add(line.Substring(2));
			}
			else if (line.StartsWith("-- ", StringComparison.Ordinal))
			{
			  logLines.Add(line.Substring(3));
			}
			else if (line.Length > 0)
			{

			  if (line.EndsWith(";", StringComparison.Ordinal))
			  {
				sqlStatement = addSqlStatementPiece(sqlStatement, line.Substring(0, line.Length - 1));
				Statement jdbcStatement = connection.createStatement();
				try
				{
				  // no logging needed as the connection will log it
				  logLines.Add(sqlStatement);
				  jdbcStatement.execute(sqlStatement);
				  jdbcStatement.close();
				}
				catch (Exception e)
				{
				  if (exception == null)
				  {
					exception = e;
					exceptionSqlStatement = sqlStatement;
				  }
				  LOG.failedDatabaseOperation(operation, sqlStatement, e);
				}
				finally
				{
				  sqlStatement = null;
				}
			  }
			  else
			  {
				sqlStatement = addSqlStatementPiece(sqlStatement, line);
			  }
			}

			line = readNextTrimmedLine(reader);
		  }
		  LOG.performingDatabaseOperation(operation, component, resourceName);
		  LOG.executingDDL(logLines);

		  if (exception != null)
		  {
			throw exception;
		  }

		  LOG.successfulDatabaseOperation(operation, component);
		}
		catch (Exception e)
		{
		  throw LOG.performDatabaseOperationException(operation, exceptionSqlStatement, e);
		}
	  }

	  protected internal virtual string addSqlStatementPiece(string sqlStatement, string line)
	  {
		if (string.ReferenceEquals(sqlStatement, null))
		{
		  return line;
		}
		return sqlStatement + " \n" + line;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected String readNextTrimmedLine(java.io.BufferedReader reader) throws java.io.IOException
	  protected internal virtual string readNextTrimmedLine(StreamReader reader)
	  {
		string line = reader.ReadLine();
		if (!string.ReferenceEquals(line, null))
		{
		  line = line.Trim();
		}
		return line;
	  }

	  protected internal virtual bool isMissingTablesException(Exception e)
	  {
		string exceptionMessage = e.Message;
		if (e.Message != null)
		{
		  // Matches message returned from H2
		  if ((exceptionMessage.IndexOf("Table", StringComparison.Ordinal) != -1) && (exceptionMessage.IndexOf("not found", StringComparison.Ordinal) != -1))
		  {
			return true;
		  }

		  // Message returned from MySQL and Oracle
		  if ((exceptionMessage.IndexOf("Table", StringComparison.Ordinal) != -1 || exceptionMessage.IndexOf("table", StringComparison.Ordinal) != -1) && (exceptionMessage.IndexOf("doesn't exist", StringComparison.Ordinal) != -1))
		  {
			return true;
		  }

		  // Message returned from Postgres
		  if ((exceptionMessage.IndexOf("relation", StringComparison.Ordinal) != -1 || exceptionMessage.IndexOf("table", StringComparison.Ordinal) != -1) && (exceptionMessage.IndexOf("does not exist", StringComparison.Ordinal) != -1))
		  {
			return true;
		  }
		}
		return false;
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual SqlSession SqlSession
	  {
		  get
		  {
			return sqlSession;
		  }
	  }
	  public virtual DbSqlSessionFactory DbSqlSessionFactory
	  {
		  get
		  {
			return dbSqlSessionFactory;
		  }
	  }

	}

}