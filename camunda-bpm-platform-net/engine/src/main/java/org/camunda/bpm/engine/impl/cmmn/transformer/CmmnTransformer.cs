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
namespace org.camunda.bpm.engine.impl.cmmn.transformer
{

	using DefaultCmmnElementHandlerRegistry = org.camunda.bpm.engine.impl.cmmn.handler.DefaultCmmnElementHandlerRegistry;
	using Transformer = org.camunda.bpm.engine.impl.core.transformer.Transformer;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CmmnTransformer : Transformer<CmmnTransform>
	{

	  protected internal ExpressionManager expressionManager;
	  protected internal CmmnTransformFactory factory;
	  protected internal IList<CmmnTransformListener> transformListeners = new List<CmmnTransformListener>();
	  protected internal DefaultCmmnElementHandlerRegistry cmmnElementHandlerRegistry;

	  public CmmnTransformer(ExpressionManager expressionManager, DefaultCmmnElementHandlerRegistry handlerRegistry, CmmnTransformFactory factory)
	  {
		this.expressionManager = expressionManager;
		this.factory = factory;
		this.cmmnElementHandlerRegistry = handlerRegistry;
	  }

	  public virtual CmmnTransform createTransform()
	  {
		return factory.createTransform(this);
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


	  public virtual CmmnTransformFactory Factory
	  {
		  get
		  {
			return factory;
		  }
		  set
		  {
			this.factory = value;
		  }
	  }


	  public virtual IList<CmmnTransformListener> TransformListeners
	  {
		  get
		  {
			return transformListeners;
		  }
		  set
		  {
			this.transformListeners = value;
		  }
	  }


	  public virtual DefaultCmmnElementHandlerRegistry CmmnElementHandlerRegistry
	  {
		  get
		  {
			return cmmnElementHandlerRegistry;
		  }
		  set
		  {
			this.cmmnElementHandlerRegistry = value;
		  }
	  }


	}

}