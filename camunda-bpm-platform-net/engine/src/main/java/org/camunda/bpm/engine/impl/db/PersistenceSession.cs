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
namespace org.camunda.bpm.engine.impl.db
{

	using BatchResult = org.apache.ibatis.executor.BatchResult;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using Session = org.camunda.bpm.engine.impl.interceptor.Session;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface PersistenceSession : Session
	{

	  // Entity Operations /////////////////////////////////

	  void executeDbOperation(DbOperation operation);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<?> selectList(String statement, Object parameter);
	  IList<object> selectList(string statement, object parameter);

	  T selectById<T>(Type<T> type, string id);

	  object selectOne(string statement, object parameter);

	  void @lock(string statement, object parameter);

	  int executeUpdate(string updateStatement, object parameter);

	  int executeNonEmptyUpdateStmt(string updateStmt, object parameter);

	  IList<BatchResult> flushOperations();

	  void commit();

	  void rollback();

	  // Schema Operations /////////////////////////////////

	  void dbSchemaCheckVersion();

	  void dbSchemaCreate();

	  void dbSchemaDrop();

	  void dbSchemaPrune();

	  void dbSchemaUpdate();

	  IList<string> TableNamesPresent {get;}

	  // listeners //////////////////////////////////////////

	  void addEntityLoadListener(EntityLoadListener listener);

	}

}