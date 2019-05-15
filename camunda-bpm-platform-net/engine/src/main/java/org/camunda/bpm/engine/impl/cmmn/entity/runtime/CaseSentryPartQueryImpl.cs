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
namespace org.camunda.bpm.engine.impl.cmmn.entity.runtime
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;

	/// <summary>
	/// This query is currently not public API on purpose.
	/// 
	/// @author Roman Smirnov
	/// </summary>
	[Serializable]
	public class CaseSentryPartQueryImpl : AbstractQuery<CaseSentryPartQueryImpl, CaseSentryPartEntity>
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseExecutionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string sentryId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string type_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string sourceCaseExecutionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string standardEvent_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string variableEvent_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string variableName_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool satisfied_Renamed;

	  public CaseSentryPartQueryImpl()
	  {
	  }

	  public CaseSentryPartQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public virtual CaseSentryPartQueryImpl caseSentryPartId(string caseSentryPartId)
	  {
		ensureNotNull(typeof(NotValidException), "caseSentryPartId", caseSentryPartId);
		this.id = caseSentryPartId;
		return this;
	  }

	  public virtual CaseSentryPartQueryImpl caseInstanceId(string caseInstanceId)
	  {
		ensureNotNull(typeof(NotValidException), "caseInstanceId", caseInstanceId);
		this.caseInstanceId_Renamed = caseInstanceId;
		return this;
	  }

	  public virtual CaseSentryPartQueryImpl caseExecutionId(string caseExecutionId)
	  {
		ensureNotNull(typeof(NotValidException), "caseExecutionId", caseExecutionId);
		this.caseExecutionId_Renamed = caseExecutionId;
		return this;
	  }

	  public virtual CaseSentryPartQueryImpl sentryId(string sentryId)
	  {
		ensureNotNull(typeof(NotValidException), "sentryId", sentryId);
		this.sentryId_Renamed = sentryId;
		return this;
	  }

	  public virtual CaseSentryPartQueryImpl type(string type)
	  {
		ensureNotNull(typeof(NotValidException), "type", type);
		this.type_Renamed = type;
		return this;
	  }

	  public virtual CaseSentryPartQueryImpl sourceCaseExecutionId(string sourceCaseExecutionId)
	  {
		ensureNotNull(typeof(NotValidException), "sourceCaseExecutionId", sourceCaseExecutionId);
		this.sourceCaseExecutionId_Renamed = sourceCaseExecutionId;
		return this;
	  }

	  public virtual CaseSentryPartQueryImpl standardEvent(string standardEvent)
	  {
		ensureNotNull(typeof(NotValidException), "standardEvent", standardEvent);
		this.standardEvent_Renamed = standardEvent;
		return this;
	  }

	  public virtual CaseSentryPartQueryImpl variableEvent(string variableEvent)
	  {
		ensureNotNull(typeof(NotValidException), "variableEvent", variableEvent);
		this.variableEvent_Renamed = variableEvent;
		return this;
	  }

	  public virtual CaseSentryPartQueryImpl variableName(string variableName)
	  {
		ensureNotNull(typeof(NotValidException), "variableName", variableName);
		this.variableName_Renamed = variableName;
		return this;
	  }

	  public virtual CaseSentryPartQueryImpl satisfied()
	  {
		this.satisfied_Renamed = true;
		return this;
	  }

	  // order by ///////////////////////////////////////////

	  public virtual CaseSentryPartQueryImpl orderByCaseSentryId()
	  {
		orderBy(CaseSentryPartQueryProperty_Fields.CASE_SENTRY_PART_ID);
		return this;
	  }

	  public virtual CaseSentryPartQueryImpl orderByCaseInstanceId()
	  {
		orderBy(CaseSentryPartQueryProperty_Fields.CASE_INSTANCE_ID);
		return this;
	  }

	  public virtual CaseSentryPartQueryImpl orderByCaseExecutionId()
	  {
		orderBy(CaseSentryPartQueryProperty_Fields.CASE_EXECUTION_ID);
		return this;
	  }

	  public virtual CaseSentryPartQueryImpl orderBySentryId()
	  {
		orderBy(CaseSentryPartQueryProperty_Fields.SENTRY_ID);
		return this;
	  }

	  public virtual CaseSentryPartQueryImpl orderBySource()
	  {
		orderBy(CaseSentryPartQueryProperty_Fields.SOURCE);
		return this;
	  }

	  // results ////////////////////////////////////////////

	  public override long executeCount(CommandContext commandContext)
	  {
		checkQueryOk();
		return commandContext.CaseSentryPartManager.findCaseSentryPartCountByQueryCriteria(this);
	  }

	  public override IList<CaseSentryPartEntity> executeList(CommandContext commandContext, Page page)
	  {
		checkQueryOk();
		IList<CaseSentryPartEntity> result = commandContext.CaseSentryPartManager.findCaseSentryPartByQueryCriteria(this, page);

		return result;
	  }

	  // getters /////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId_Renamed;
		  }
	  }

	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId_Renamed;
		  }
	  }

	  public virtual string SentryId
	  {
		  get
		  {
			return sentryId_Renamed;
		  }
	  }

	  public virtual string Type
	  {
		  get
		  {
			return type_Renamed;
		  }
	  }

	  public virtual string SourceCaseExecutionId
	  {
		  get
		  {
			return sourceCaseExecutionId_Renamed;
		  }
	  }

	  public virtual string StandardEvent
	  {
		  get
		  {
			return standardEvent_Renamed;
		  }
	  }

	  public virtual string VariableEvent
	  {
		  get
		  {
			return variableEvent_Renamed;
		  }
	  }

	  public virtual string VariableName
	  {
		  get
		  {
			return variableName_Renamed;
		  }
	  }

	  public virtual bool Satisfied
	  {
		  get
		  {
			return satisfied_Renamed;
		  }
	  }

	}

}