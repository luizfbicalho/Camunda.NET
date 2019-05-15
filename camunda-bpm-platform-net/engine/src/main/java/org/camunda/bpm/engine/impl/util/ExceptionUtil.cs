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
namespace org.camunda.bpm.engine.impl.util
{

	using BatchExecutorException = org.apache.ibatis.executor.BatchExecutorException;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using ResourceType = org.camunda.bpm.engine.repository.ResourceType;

	/// <summary>
	/// @author Roman Smirnov
	/// @author Askar Akhmerov
	/// </summary>
	public class ExceptionUtil
	{

	  public static string getExceptionStacktrace(Exception exception)
	  {
		StringWriter stringWriter = new StringWriter();
		exception.printStackTrace(new PrintWriter(stringWriter));
		return stringWriter.ToString();
	  }

	  public static string getExceptionStacktrace(ByteArrayEntity byteArray)
	  {
		string result = null;
		if (byteArray != null)
		{
		  result = StringUtil.fromBytes(byteArray.Bytes);
		}
		return result;
	  }

	  public static ByteArrayEntity createJobExceptionByteArray(sbyte[] byteArray, ResourceType type)
	  {
		return createExceptionByteArray("job.exceptionByteArray", byteArray, type);
	  }

	  /// <summary>
	  /// create ByteArrayEntity with specified name and payload and make sure it's
	  /// persisted
	  /// 
	  /// used in Jobs and ExternalTasks
	  /// </summary>
	  /// <param name="name"> - type\source of the exception </param>
	  /// <param name="byteArray"> - payload of the exception </param>
	  /// <param name="type"> - resource type of the exception </param>
	  /// <returns> persisted entity </returns>
	  public static ByteArrayEntity createExceptionByteArray(string name, sbyte[] byteArray, ResourceType type)
	  {
		ByteArrayEntity result = null;

		if (byteArray != null)
		{
		  result = new ByteArrayEntity(name, byteArray, type);
		  Context.CommandContext.ByteArrayManager.insertByteArray(result);
		}

		return result;
	  }

	  public static bool checkValueTooLongException(ProcessEngineException exception)
	  {
		IList<SQLException> sqlExceptionList = findRelatedSqlExceptions(exception);
		foreach (SQLException ex in sqlExceptionList)
		{
		  if (ex.Message.contains("too long") || ex.Message.contains("too large") || ex.Message.contains("TOO LARGE") || ex.Message.contains("ORA-01461") || ex.Message.contains("ORA-01401") || ex.Message.contains("data would be truncated") || ex.Message.contains("SQLCODE=-302, SQLSTATE=22001"))
		  {
			return true;
		  }
		}
		return false;
	  }

	  public static bool checkConstraintViolationException(ProcessEngineException exception)
	  {
		IList<SQLException> sqlExceptionList = findRelatedSqlExceptions(exception);
		foreach (SQLException ex in sqlExceptionList)
		{
		  if (ex.Message.contains("constraint") || ex.Message.contains("violat") || ex.Message.ToLower().Contains("duplicate") || ex.Message.contains("ORA-00001") || ex.Message.contains("SQLCODE=-803, SQLSTATE=23505"))
		  {
			return true;
		  }
		}
		return false;
	  }

	  public static IList<SQLException> findRelatedSqlExceptions(Exception exception)
	  {
		IList<SQLException> sqlExceptionList = new List<SQLException>();
		Exception cause = exception;
		do
		{
		  if (cause is SQLException)
		  {
			SQLException sqlEx = (SQLException) cause;
			sqlExceptionList.Add(sqlEx);
			while (sqlEx.NextException != null)
			{
			  sqlExceptionList.Add(sqlEx.NextException);
			  sqlEx = sqlEx.NextException;
			}
		  }
		  cause = cause.InnerException;
		} while (cause != null);
		return sqlExceptionList;
	  }

	  public static bool checkForeignKeyConstraintViolation(Exception cause)
	  {

		IList<SQLException> relatedSqlExceptions = findRelatedSqlExceptions(cause);
		foreach (SQLException exception in relatedSqlExceptions)
		{

		  // PostgreSQL doesn't allow for a proper check
		  if ("23503".Equals(exception.SQLState) && exception.ErrorCode == 0)
		  {
			return false;
		  }
		  else if ((exception.Message.ToLower().Contains("foreign key constraint") || ("23000".Equals(exception.SQLState) && exception.ErrorCode == 547)) || (exception.Message.ToLower().Contains("foreign key constraint") || ("23000".Equals(exception.SQLState) && exception.ErrorCode == 1452)) || (exception.Message.ToLower().Contains("integrity constraint") || ("23000".Equals(exception.SQLState) && exception.ErrorCode == 2291) || ("23506".Equals(exception.SQLState) && exception.ErrorCode == 23506)) || (exception.Message.ToLower().Contains("sqlstate=23503") && exception.Message.ToLower().Contains("sqlcode=-530")) || ("23503".Equals(exception.SQLState) && exception.ErrorCode == -530))
		  {

			return true;
		  }
		}

		return false;
	  }

	  public static bool checkVariableIntegrityViolation(Exception cause)
	  {

		IList<SQLException> relatedSqlExceptions = findRelatedSqlExceptions(cause);
		foreach (SQLException exception in relatedSqlExceptions)
		{
		  if ((exception.Message.ToLower().Contains("act_uniq_variable") && "23000".Equals(exception.SQLState) && exception.ErrorCode == 1062) || (exception.Message.ToLower().Contains("act_uniq_variable") && "23505".Equals(exception.SQLState) && exception.ErrorCode == 0) || (exception.Message.ToLower().Contains("act_uniq_variable") && "23000".Equals(exception.SQLState) && exception.ErrorCode == 2601) || (exception.Message.ToLower().Contains("act_uniq_variable") && "23000".Equals(exception.SQLState) && exception.ErrorCode == 1) || (exception.Message.ToLower().Contains("act_uniq_variable_index_c") && "23505".Equals(exception.SQLState) && exception.ErrorCode == 23505))
		  {
			return true;
		  }
		}

		return false;
	  }

	  public static BatchExecutorException findBatchExecutorException(Exception exception)
	  {
		Exception cause = exception;
		do
		{
		  if (cause is BatchExecutorException)
		  {
			return (BatchExecutorException) cause;
		  }
		  cause = cause.InnerException;
		} while (cause != null);

		return null;
	  }
	}

}