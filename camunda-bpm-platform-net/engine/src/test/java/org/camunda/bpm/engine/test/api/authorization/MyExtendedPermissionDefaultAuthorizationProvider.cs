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
namespace org.camunda.bpm.engine.test.api.authorization
{
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using DefaultAuthorizationProvider = org.camunda.bpm.engine.impl.cfg.auth.DefaultAuthorizationProvider;
	using AuthorizationEntity = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationEntity;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Johannes Heinemann
	/// </summary>
	public class MyExtendedPermissionDefaultAuthorizationProvider : DefaultAuthorizationProvider
	{

	  public override AuthorizationEntity[] newTaskAssignee(Task task, string oldAssignee, string newAssignee)
	  {
		AuthorizationEntity[] authorizations = base.newTaskAssignee(task, oldAssignee, newAssignee);
		authorizations[0].addPermission(Permissions.DELETE);
		return authorizations;
	  }
	}

}