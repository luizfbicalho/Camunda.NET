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
namespace org.camunda.bpm.engine.impl.cmd
{

	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AuthorizationManager = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationManager;
	using PropertyEntity = org.camunda.bpm.engine.impl.persistence.entity.PropertyEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class GetPropertiesCmd : Command<IDictionary<string, string>>
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.Map<String, String> execute(org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public virtual IDictionary<string, string> execute(CommandContext commandContext)
	  {
		AuthorizationManager authorizationManager = commandContext.AuthorizationManager;
		authorizationManager.checkCamundaAdmin();

		IList<PropertyEntity> propertyEntities = commandContext.DbEntityManager.selectList("selectProperties");

		IDictionary<string, string> properties = new Dictionary<string, string>();
		foreach (PropertyEntity propertyEntity in propertyEntities)
		{
		  properties[propertyEntity.Name] = propertyEntity.Value;
		}
		return properties;
	  }

	}

}