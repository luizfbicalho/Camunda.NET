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
namespace org.camunda.bpm.engine.cdi.impl.el
{


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ElContextDelegate : ELContext
	{

	  protected internal readonly org.camunda.bpm.engine.impl.javax.el.ELContext delegateContext;

	  protected internal readonly ELResolver elResolver;

	  public ElContextDelegate(org.camunda.bpm.engine.impl.javax.el.ELContext delegateContext, ELResolver elResolver)
	  {
		this.delegateContext = delegateContext;
		this.elResolver = elResolver;
	  }

	  public virtual ELResolver ELResolver
	  {
		  get
		  {
			return elResolver;
		  }
	  }

	  public virtual FunctionMapper FunctionMapper
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual VariableMapper VariableMapper
	  {
		  get
		  {
			return null;
		  }
	  }

	  // delegate methods ////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public Object getContext(Class key)
	  public virtual object getContext(Type key)
	  {
		return delegateContext.getContext(key);
	  }

	  public override bool Equals(object obj)
	  {
		return delegateContext.Equals(obj);
	  }

	  public virtual Locale Locale
	  {
		  get
		  {
			return delegateContext.Locale;
		  }
		  set
		  {
			delegateContext.Locale = value;
		  }
	  }

	  public virtual bool PropertyResolved
	  {
		  get
		  {
			return delegateContext.PropertyResolved;
		  }
		  set
		  {
			delegateContext.PropertyResolved = value;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public void putContext(Class key, Object contextObject)
	  public virtual void putContext(Type key, object contextObject)
	  {
		delegateContext.putContext(key, contextObject);
	  }



	}

}