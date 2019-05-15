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

	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AuthorizationManager = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationManager;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using PropertyEntity = org.camunda.bpm.engine.impl.persistence.entity.PropertyEntity;
	using PropertyManager = org.camunda.bpm.engine.impl.persistence.entity.PropertyManager;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DeletePropertyCmd : Command<object>
	{

	  protected internal string name;

	  /// <param name="name"> </param>
	  public DeletePropertyCmd(string name)
	  {
		this.name = name;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		AuthorizationManager authorizationManager = commandContext.AuthorizationManager;
		authorizationManager.checkCamundaAdmin();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.PropertyManager propertyManager = commandContext.getPropertyManager();
		PropertyManager propertyManager = commandContext.PropertyManager;

		PropertyEntity propertyEntity = propertyManager.findPropertyById(name);

		if (propertyEntity != null)
		{
		  propertyManager.delete(propertyEntity);

		  commandContext.OperationLogManager.logPropertyOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, Collections.singletonList(new PropertyChange("name", null, name)));
		}

		return null;
	  }

	}

}