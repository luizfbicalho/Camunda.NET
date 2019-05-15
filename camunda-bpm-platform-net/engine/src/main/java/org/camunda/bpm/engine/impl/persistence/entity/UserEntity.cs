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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using PasswordPolicyResult = org.camunda.bpm.engine.identity.PasswordPolicyResult;
	using User = org.camunda.bpm.engine.identity.User;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EncryptionUtil.saltPassword;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class UserEntity : User, DbEntity, HasDbRevision
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal int revision;
	  protected internal string firstName;
	  protected internal string lastName;
	  protected internal string email;
	  protected internal string password;
	  protected internal string newPassword;
	  protected internal string salt;
	  protected internal DateTime lockExpirationTime;
	  protected internal int attempts;

	  public UserEntity()
	  {
	  }

	  public UserEntity(string id)
	  {
		this.id = id;
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["firstName"] = firstName;
			persistentState["lastName"] = lastName;
			persistentState["email"] = email;
			persistentState["password"] = password;
			persistentState["salt"] = salt;
			return persistentState;
		  }
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }
	  public virtual string FirstName
	  {
		  get
		  {
			return firstName;
		  }
		  set
		  {
			this.firstName = value;
		  }
	  }
	  public virtual string LastName
	  {
		  get
		  {
			return lastName;
		  }
		  set
		  {
			this.lastName = value;
		  }
	  }
	  public virtual string Email
	  {
		  get
		  {
			return email;
		  }
		  set
		  {
			this.email = value;
		  }
	  }
	  public virtual string Password
	  {
		  get
		  {
			return password;
		  }
		  set
		  {
			this.newPassword = value;
		  }
	  }

	  public virtual string Salt
	  {
		  get
		  {
			return this.salt;
		  }
		  set
		  {
			this.salt = value;
		  }
	  }

	  /// <summary>
	  /// Special setter for MyBatis.
	  /// </summary>
	  public virtual string DbPassword
	  {
		  set
		  {
			this.password = value;
		  }
	  }

	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }

	  public virtual DateTime LockExpirationTime
	  {
		  get
		  {
			return lockExpirationTime;
		  }
		  set
		  {
			this.lockExpirationTime = value;
		  }
	  }


	  public virtual int Attempts
	  {
		  get
		  {
			return attempts;
		  }
		  set
		  {
			this.attempts = value;
		  }
	  }


	  public virtual void encryptPassword()
	  {
		if (!string.ReferenceEquals(newPassword, null))
		{
		  salt = generateSalt();
		  DbPassword = encryptPassword(newPassword, salt);
		}
	  }

	  protected internal virtual string encryptPassword(string password, string salt)
	  {
		if (string.ReferenceEquals(password, null))
		{
		  return null;
		}
		else
		{
		  string saltedPassword = saltPassword(password, salt);
		  return Context.ProcessEngineConfiguration.PasswordManager.encrypt(saltedPassword);
		}
	  }

	  protected internal virtual string generateSalt()
	  {
		return Context.ProcessEngineConfiguration.SaltGenerator.generateSalt();
	  }


	  public virtual bool checkPasswordAgainstPolicy()
	  {
		PasswordPolicyResult result = Context.ProcessEngineConfiguration.IdentityService.checkPasswordAgainstPolicy(newPassword);

		return result.Valid;
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[id=" + id + ", revision=" + revision + ", firstName=" + firstName + ", lastName=" + lastName + ", email=" + email + ", password=" + password + ", salt=" + salt + ", lockExpirationTime=" + lockExpirationTime + ", attempts=" + attempts + "]";
	  }
	}

}