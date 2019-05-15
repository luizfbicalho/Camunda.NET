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
namespace org.camunda.bpm.engine.impl.cmmn.execution
{

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public abstract class CmmnSentryPart
	{

	  private const long serialVersionUID = 1L;

	  protected internal string type;
	  protected internal string sentryId;
	  protected internal string standardEvent;
	  protected internal string source;
	  protected internal string variableEvent;
	  protected internal string variableName;
	  protected internal bool satisfied = false;

	  public abstract CmmnExecution CaseInstance {get;set;}


	  public abstract CmmnExecution CaseExecution {get;set;}


	  public virtual string SentryId
	  {
		  get
		  {
			return sentryId;
		  }
		  set
		  {
			this.sentryId = value;
		  }
	  }


	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
		  set
		  {
			this.type = value;
		  }
	  }


	  public virtual string Source
	  {
		  get
		  {
			return source;
		  }
		  set
		  {
			this.source = value;
		  }
	  }


	  /// @deprecated since 7.4 A new instance of a sentry
	  /// does not reference the source case execution id anymore. 
	  public abstract string SourceCaseExecutionId {get;}

	  /// @deprecated since 7.4 A new instance of a sentry
	  /// does not reference the source case execution id anymore. 
	  public abstract CmmnExecution SourceCaseExecution {get;set;}


	  public virtual string StandardEvent
	  {
		  get
		  {
			return standardEvent;
		  }
		  set
		  {
			this.standardEvent = value;
		  }
	  }


	  public virtual bool Satisfied
	  {
		  get
		  {
			return satisfied;
		  }
		  set
		  {
			this.satisfied = value;
		  }
	  }


	  public virtual string VariableEvent
	  {
		  get
		  {
			return variableEvent;
		  }
		  set
		  {
			this.variableEvent = value;
		  }
	  }


	  public virtual string VariableName
	  {
		  get
		  {
			return variableName;
		  }
		  set
		  {
			this.variableName = value;
		  }
	  }


	}

}