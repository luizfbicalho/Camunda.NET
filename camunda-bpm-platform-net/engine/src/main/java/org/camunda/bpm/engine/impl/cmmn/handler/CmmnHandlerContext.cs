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
namespace org.camunda.bpm.engine.impl.cmmn.handler
{
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using HandlerContext = org.camunda.bpm.engine.impl.core.handler.HandlerContext;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;

	/// <summary>
	/// <para>This context contains necessary information (like caseDefinition)
	/// to be accessed by a <seealso cref="CmmnHandlerContext"/>.</para>
	/// 
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CmmnHandlerContext : HandlerContext
	{

	  protected internal ExpressionManager expressionManager;
	  protected internal CmmnCaseDefinition caseDefinition;
	  protected internal CmmnModelInstance model;
	  protected internal CmmnActivity parent;
	  protected internal Deployment deployment;

	  public CmmnHandlerContext()
	  {
	  }

	  public virtual CmmnModelInstance Model
	  {
		  get
		  {
			return model;
		  }
		  set
		  {
			this.model = value;
		  }
	  }


	  public virtual CmmnCaseDefinition CaseDefinition
	  {
		  get
		  {
			return caseDefinition;
		  }
		  set
		  {
			this.caseDefinition = value;
		  }
	  }


	  public virtual CmmnActivity Parent
	  {
		  get
		  {
			return parent;
		  }
		  set
		  {
			this.parent = value;
		  }
	  }


	  public virtual Deployment Deployment
	  {
		  get
		  {
			return deployment;
		  }
		  set
		  {
			this.deployment = value;
		  }
	  }


	  public virtual ExpressionManager ExpressionManager
	  {
		  get
		  {
			return expressionManager;
		  }
		  set
		  {
			this.expressionManager = value;
		  }
	  }


	}

}