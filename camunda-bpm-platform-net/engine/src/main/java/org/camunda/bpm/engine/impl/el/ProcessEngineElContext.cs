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
namespace org.camunda.bpm.engine.impl.el
{

	using ELContext = org.camunda.bpm.engine.impl.javax.el.ELContext;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;
	using FunctionMapper = org.camunda.bpm.engine.impl.javax.el.FunctionMapper;
	using VariableMapper = org.camunda.bpm.engine.impl.javax.el.VariableMapper;


	/// <summary>
	/// <seealso cref="ELContext"/> used by the process engine.
	/// 
	/// @author Joram Barrez
	/// @author Daniel Meyer
	/// </summary>
	public class ProcessEngineElContext : ELContext
	{

	  protected internal ELResolver elResolver;

	  protected internal FunctionMapper functionMapper;

	  public ProcessEngineElContext(IList<FunctionMapper> functionMappers, ELResolver elResolver) : this(functionMappers)
	  {
		this.elResolver = elResolver;
	  }


	  public ProcessEngineElContext(IList<FunctionMapper> functionMappers)
	  {
		this.functionMapper = new CompositeFunctionMapper(functionMappers);
	  }

	  public override ELResolver ELResolver
	  {
		  get
		  {
			return elResolver;
		  }
	  }

	  public override FunctionMapper FunctionMapper
	  {
		  get
		  {
			return functionMapper;
		  }
	  }

	  public override VariableMapper VariableMapper
	  {
		  get
		  {
			return null;
		  }
	  }

	}

}