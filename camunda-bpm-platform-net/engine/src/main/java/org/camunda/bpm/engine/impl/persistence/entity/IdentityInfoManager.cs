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

	using Context = org.camunda.bpm.engine.impl.context.Context;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class IdentityInfoManager : AbstractManager
	{

	  public virtual void deleteUserInfoByUserIdAndKey(string userId, string key)
	  {
		IdentityInfoEntity identityInfoEntity = findUserInfoByUserIdAndKey(userId, key);
		if (identityInfoEntity != null)
		{
		  deleteIdentityInfo(identityInfoEntity);
		}
	  }

	  public virtual void deleteIdentityInfo(IdentityInfoEntity identityInfo)
	  {
		DbEntityManager.delete(identityInfo);
		if (IdentityInfoEntity.TYPE_USERACCOUNT.Equals(identityInfo.Type))
		{
		  foreach (IdentityInfoEntity identityInfoDetail in findIdentityInfoDetails(identityInfo.Id))
		  {
			DbEntityManager.delete(identityInfoDetail);
		  }
		}
	  }

	  public virtual IdentityInfoEntity findUserAccountByUserIdAndKey(string userId, string userPassword, string key)
	  {
		IdentityInfoEntity identityInfoEntity = findUserInfoByUserIdAndKey(userId, key);
		if (identityInfoEntity == null)
		{
		  return null;
		}

		IDictionary<string, string> details = new Dictionary<string, string>();
		string identityInfoId = identityInfoEntity.Id;
		IList<IdentityInfoEntity> identityInfoDetails = findIdentityInfoDetails(identityInfoId);
		foreach (IdentityInfoEntity identityInfoDetail in identityInfoDetails)
		{
		  details[identityInfoDetail.Key] = identityInfoDetail.Value;
		}
		identityInfoEntity.Details = details;

		if (identityInfoEntity.PasswordBytes != null)
		{
		  string password = decryptPassword(identityInfoEntity.PasswordBytes, userPassword);
		  identityInfoEntity.Password = password;
		}

		return identityInfoEntity;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected java.util.List<IdentityInfoEntity> findIdentityInfoDetails(String identityInfoId)
	  protected internal virtual IList<IdentityInfoEntity> findIdentityInfoDetails(string identityInfoId)
	  {
		return Context.CommandContext.DbSqlSession.SqlSession.selectList("selectIdentityInfoDetails", identityInfoId);
	  }

	  public virtual void setUserInfo(string userId, string userPassword, string type, string key, string value, string accountPassword, IDictionary<string, string> accountDetails)
	  {
		sbyte[] storedPassword = null;
		if (!string.ReferenceEquals(accountPassword, null))
		{
		  storedPassword = encryptPassword(accountPassword, userPassword);
		}

		IdentityInfoEntity identityInfoEntity = findUserInfoByUserIdAndKey(userId, key);
		if (identityInfoEntity != null)
		{
		  // update
		  identityInfoEntity.Value = value;
		  identityInfoEntity.PasswordBytes = storedPassword;

		  if (accountDetails == null)
		  {
			accountDetails = new Dictionary<string, string>();
		  }

		  ISet<string> newKeys = new HashSet<string>(accountDetails.Keys);
		  IList<IdentityInfoEntity> identityInfoDetails = findIdentityInfoDetails(identityInfoEntity.Id);
		  foreach (IdentityInfoEntity identityInfoDetail in identityInfoDetails)
		  {
			string detailKey = identityInfoDetail.Key;
			newKeys.remove(detailKey);
			string newDetailValue = accountDetails[detailKey];
			if (string.ReferenceEquals(newDetailValue, null))
			{
			  deleteIdentityInfo(identityInfoDetail);
			}
			else
			{
			  // update detail
			  identityInfoDetail.Value = newDetailValue;
			}
		  }
		  insertAccountDetails(identityInfoEntity, accountDetails, newKeys);


		}
		else
		{
		  // insert
		  identityInfoEntity = new IdentityInfoEntity();
		  identityInfoEntity.UserId = userId;
		  identityInfoEntity.Type = type;
		  identityInfoEntity.Key = key;
		  identityInfoEntity.Value = value;
		  identityInfoEntity.PasswordBytes = storedPassword;
		  DbEntityManager.insert(identityInfoEntity);
		  if (accountDetails != null)
		  {
			insertAccountDetails(identityInfoEntity, accountDetails, accountDetails.Keys);
		  }
		}
	  }

	  private void insertAccountDetails(IdentityInfoEntity identityInfoEntity, IDictionary<string, string> accountDetails, ISet<string> keys)
	  {
		foreach (string newKey in keys)
		{
		  // insert detail
		  IdentityInfoEntity identityInfoDetail = new IdentityInfoEntity();
		  identityInfoDetail.ParentId = identityInfoEntity.Id;
		  identityInfoDetail.Key = newKey;
		  identityInfoDetail.Value = accountDetails[newKey];
		  DbEntityManager.insert(identityInfoDetail);
		}
	  }

	  public virtual sbyte[] encryptPassword(string accountPassword, string userPassword)
	  {
		// TODO
		return accountPassword.GetBytes();
	  }

	  public virtual string decryptPassword(sbyte[] storedPassword, string userPassword)
	  {
		// TODO
		return StringHelper.NewString(storedPassword);
	  }

	  public virtual IdentityInfoEntity findUserInfoByUserIdAndKey(string userId, string key)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["userId"] = userId;
		parameters["key"] = key;
		return (IdentityInfoEntity) DbEntityManager.selectOne("selectIdentityInfoByUserIdAndKey", parameters);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<String> findUserInfoKeysByUserIdAndType(String userId, String type)
	  public virtual IList<string> findUserInfoKeysByUserIdAndType(string userId, string type)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["userId"] = userId;
		parameters["type"] = type;
		return (System.Collections.IList) DbSqlSession.SqlSession.selectList("selectIdentityInfoKeysByUserIdAndType", parameters);
	  }

	  public virtual void deleteUserInfoByUserId(string userId)
	  {
		IList<IdentityInfoEntity> identityInfos = DbEntityManager.selectList("selectIdentityInfoByUserId", userId);
		foreach (IdentityInfoEntity identityInfo in identityInfos)
		{
		  IdentityInfoManager.deleteIdentityInfo(identityInfo);
		}
	  }

	  public virtual void updateUserLock(UserEntity user, int attempts, DateTime lockExpirationTime)
	  {
		user.Attempts = attempts;
		user.LockExpirationTime = lockExpirationTime;
		DbEntityManager.update(typeof(UserEntity), "updateUserLock", user);
	  }
	}

}