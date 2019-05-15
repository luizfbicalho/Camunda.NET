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
	using IdentityInfoEntity = org.camunda.bpm.engine.impl.persistence.entity.IdentityInfoEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class SetUserInfoCmd : Command<object>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string userId;
	  protected internal string userPassword;
	  protected internal string type;
	  protected internal string key;
	  protected internal string value;
	  protected internal string accountPassword;
	  protected internal IDictionary<string, string> accountDetails;

	  public SetUserInfoCmd(string userId, string key, string value)
	  {
		this.userId = userId;
		this.type = IdentityInfoEntity.TYPE_USERINFO;
		this.key = key;
		this.value = value;
	  }

	  public SetUserInfoCmd(string userId, string userPassword, string accountName, string accountUsername, string accountPassword, IDictionary<string, string> accountDetails)
	  {
		this.userId = userId;
		this.userPassword = userPassword;
		this.type = IdentityInfoEntity.TYPE_USERACCOUNT;
		this.key = accountName;
		this.value = accountUsername;
		this.accountPassword = accountPassword;
		this.accountDetails = accountDetails;
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		commandContext.IdentityInfoManager.setUserInfo(userId, userPassword, type, key, value, accountPassword, accountDetails);
		return null;
	  }
	}

}