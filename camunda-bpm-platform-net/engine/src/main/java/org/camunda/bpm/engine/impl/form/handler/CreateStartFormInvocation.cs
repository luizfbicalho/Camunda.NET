﻿/*
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
namespace org.camunda.bpm.engine.impl.form.handler
{
	using DelegateInvocation = org.camunda.bpm.engine.impl.@delegate.DelegateInvocation;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CreateStartFormInvocation : DelegateInvocation
	{

	  protected internal StartFormHandler startFormHandler;
	  protected internal ProcessDefinitionEntity definition;

	  public CreateStartFormInvocation(StartFormHandler startFormHandler, ProcessDefinitionEntity definition) : base(null, definition)
	  {
		this.startFormHandler = startFormHandler;
		this.definition = definition;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void invoke() throws Exception
	  protected internal override void invoke()
	  {
		invocationResult = startFormHandler.createStartFormData(definition);
	  }

	}

}