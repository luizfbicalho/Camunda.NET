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
namespace org.camunda.bpm.engine.impl
{

	using PasswordPolicyResult = org.camunda.bpm.engine.identity.PasswordPolicyResult;
	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using NativeUserQuery = org.camunda.bpm.engine.identity.NativeUserQuery;
	using PasswordPolicy = org.camunda.bpm.engine.identity.PasswordPolicy;
	using PasswordPolicyRule = org.camunda.bpm.engine.identity.PasswordPolicyRule;
	using Picture = org.camunda.bpm.engine.identity.Picture;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using TenantQuery = org.camunda.bpm.engine.identity.TenantQuery;
	using User = org.camunda.bpm.engine.identity.User;
	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
	using CheckPassword = org.camunda.bpm.engine.impl.cmd.CheckPassword;
	using GetPasswordPolicyCmd = org.camunda.bpm.engine.impl.cmd.GetPasswordPolicyCmd;
	using CreateGroupCmd = org.camunda.bpm.engine.impl.cmd.CreateGroupCmd;
	using CreateGroupQueryCmd = org.camunda.bpm.engine.impl.cmd.CreateGroupQueryCmd;
	using CreateMembershipCmd = org.camunda.bpm.engine.impl.cmd.CreateMembershipCmd;
	using CreateNativeUserQueryCmd = org.camunda.bpm.engine.impl.cmd.CreateNativeUserQueryCmd;
	using CreateTenantCmd = org.camunda.bpm.engine.impl.cmd.CreateTenantCmd;
	using CreateTenantGroupMembershipCmd = org.camunda.bpm.engine.impl.cmd.CreateTenantGroupMembershipCmd;
	using CreateTenantQueryCmd = org.camunda.bpm.engine.impl.cmd.CreateTenantQueryCmd;
	using CreateTenantUserMembershipCmd = org.camunda.bpm.engine.impl.cmd.CreateTenantUserMembershipCmd;
	using CreateUserCmd = org.camunda.bpm.engine.impl.cmd.CreateUserCmd;
	using CreateUserQueryCmd = org.camunda.bpm.engine.impl.cmd.CreateUserQueryCmd;
	using DeleteGroupCmd = org.camunda.bpm.engine.impl.cmd.DeleteGroupCmd;
	using DeleteMembershipCmd = org.camunda.bpm.engine.impl.cmd.DeleteMembershipCmd;
	using DeleteTenantCmd = org.camunda.bpm.engine.impl.cmd.DeleteTenantCmd;
	using DeleteTenantGroupMembershipCmd = org.camunda.bpm.engine.impl.cmd.DeleteTenantGroupMembershipCmd;
	using DeleteTenantUserMembershipCmd = org.camunda.bpm.engine.impl.cmd.DeleteTenantUserMembershipCmd;
	using DeleteUserCmd = org.camunda.bpm.engine.impl.cmd.DeleteUserCmd;
	using DeleteUserInfoCmd = org.camunda.bpm.engine.impl.cmd.DeleteUserInfoCmd;
	using DeleteUserPictureCmd = org.camunda.bpm.engine.impl.cmd.DeleteUserPictureCmd;
	using GetUserAccountCmd = org.camunda.bpm.engine.impl.cmd.GetUserAccountCmd;
	using GetUserInfoCmd = org.camunda.bpm.engine.impl.cmd.GetUserInfoCmd;
	using GetUserInfoKeysCmd = org.camunda.bpm.engine.impl.cmd.GetUserInfoKeysCmd;
	using GetUserPictureCmd = org.camunda.bpm.engine.impl.cmd.GetUserPictureCmd;
	using IsIdentityServiceReadOnlyCmd = org.camunda.bpm.engine.impl.cmd.IsIdentityServiceReadOnlyCmd;
	using SaveGroupCmd = org.camunda.bpm.engine.impl.cmd.SaveGroupCmd;
	using SaveTenantCmd = org.camunda.bpm.engine.impl.cmd.SaveTenantCmd;
	using SaveUserCmd = org.camunda.bpm.engine.impl.cmd.SaveUserCmd;
	using SetUserInfoCmd = org.camunda.bpm.engine.impl.cmd.SetUserInfoCmd;
	using SetUserPictureCmd = org.camunda.bpm.engine.impl.cmd.SetUserPictureCmd;
	using UnlockUserCmd = org.camunda.bpm.engine.impl.cmd.UnlockUserCmd;
	using Account = org.camunda.bpm.engine.impl.identity.Account;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using PasswordPolicyResultImpl = org.camunda.bpm.engine.impl.identity.PasswordPolicyResultImpl;
	using GroupEntity = org.camunda.bpm.engine.impl.persistence.entity.GroupEntity;
	using IdentityInfoEntity = org.camunda.bpm.engine.impl.persistence.entity.IdentityInfoEntity;
	using EnsureUtil = org.camunda.bpm.engine.impl.util.EnsureUtil;
	using ExceptionUtil = org.camunda.bpm.engine.impl.util.ExceptionUtil;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class IdentityServiceImpl : ServiceImpl, IdentityService
	{

	  /// <summary>
	  /// thread local holding the current authentication </summary>
	  private ThreadLocal<Authentication> currentAuthentication = new ThreadLocal<Authentication>();

	  public virtual bool ReadOnly
	  {
		  get
		  {
			return commandExecutor.execute(new IsIdentityServiceReadOnlyCmd());
		  }
	  }

	  public virtual Group newGroup(string groupId)
	  {
		return commandExecutor.execute(new CreateGroupCmd(groupId));
	  }

	  public virtual User newUser(string userId)
	  {
		return commandExecutor.execute(new CreateUserCmd(userId));
	  }

	  public virtual Tenant newTenant(string tenantId)
	  {
		return commandExecutor.execute(new CreateTenantCmd(tenantId));
	  }

	  public virtual void saveGroup(Group group)
	  {

		try
		{
		  commandExecutor.execute(new SaveGroupCmd((GroupEntity) group));
		}
		catch (ProcessEngineException ex)
		{
		  if (ExceptionUtil.checkConstraintViolationException(ex))
		  {
			throw new BadUserRequestException("The group already exists", ex);
		  }
		  throw ex;
		}
	  }

	  public virtual void saveUser(User user)
	  {
		saveUser(user, false);
	  }

	  public virtual void saveUser(User user, bool skipPasswordPolicy)
	  {
		try
		{
		  commandExecutor.execute(new SaveUserCmd(user, skipPasswordPolicy));
		}
		catch (ProcessEngineException ex)
		{
		  if (ExceptionUtil.checkConstraintViolationException(ex))
		  {
			throw new BadUserRequestException("The user already exists", ex);
		  }
		  throw ex;
		}
	  }

	  public virtual void saveTenant(Tenant tenant)
	  {

		try
		{
		  commandExecutor.execute(new SaveTenantCmd(tenant));
		}
		catch (ProcessEngineException ex)
		{
		  if (ExceptionUtil.checkConstraintViolationException(ex))
		  {
			throw new BadUserRequestException("The tenant already exists", ex);
		  }
		  throw ex;
		}
	  }

	  public virtual UserQuery createUserQuery()
	  {
		return commandExecutor.execute(new CreateUserQueryCmd());
	  }

	  public virtual NativeUserQuery createNativeUserQuery()
	  {
		return commandExecutor.execute(new CreateNativeUserQueryCmd());
	  }

	  public virtual GroupQuery createGroupQuery()
	  {
		return commandExecutor.execute(new CreateGroupQueryCmd());
	  }

	  public virtual TenantQuery createTenantQuery()
	  {
		return commandExecutor.execute(new CreateTenantQueryCmd());
	  }

	  public virtual void createMembership(string userId, string groupId)
	  {
		commandExecutor.execute(new CreateMembershipCmd(userId, groupId));
	  }

	  public virtual void deleteGroup(string groupId)
	  {
		commandExecutor.execute(new DeleteGroupCmd(groupId));
	  }

	  public virtual void deleteMembership(string userId, string groupId)
	  {
		commandExecutor.execute(new DeleteMembershipCmd(userId, groupId));
	  }

	  public virtual bool checkPassword(string userId, string password)
	  {
		return commandExecutor.execute(new CheckPassword(userId, password));
	  }

	  public virtual PasswordPolicyResult checkPasswordAgainstPolicy(string password)
	  {
		return checkPasswordAgainstPolicy(PasswordPolicy, password);
	  }

	  public virtual PasswordPolicyResult checkPasswordAgainstPolicy(PasswordPolicy policy, string password)
	  {
		EnsureUtil.ensureNotNull("policy", policy);
		EnsureUtil.ensureNotNull("password", password);

		IList<PasswordPolicyRule> violatedRules = new List<PasswordPolicyRule>();
		IList<PasswordPolicyRule> fulfilledRules = new List<PasswordPolicyRule>();

		foreach (PasswordPolicyRule rule in policy.Rules)
		{
		  if (rule.execute(password))
		  {
			fulfilledRules.Add(rule);
		  }
		  else
		  {
			violatedRules.Add(rule);
		  }
		}
		return new PasswordPolicyResultImpl(violatedRules, fulfilledRules);
	  }

	  public virtual PasswordPolicy PasswordPolicy
	  {
		  get
		  {
			return commandExecutor.execute(new GetPasswordPolicyCmd());
		  }
	  }

	  public virtual void unlockUser(string userId)
	  {
		commandExecutor.execute(new UnlockUserCmd(userId));
	  }

	  public virtual void deleteUser(string userId)
	  {
		commandExecutor.execute(new DeleteUserCmd(userId));
	  }

	  public virtual void deleteTenant(string tenantId)
	  {
		commandExecutor.execute(new DeleteTenantCmd(tenantId));
	  }

	  public virtual void setUserPicture(string userId, Picture picture)
	  {
		commandExecutor.execute(new SetUserPictureCmd(userId, picture));
	  }

	  public virtual Picture getUserPicture(string userId)
	  {
		return commandExecutor.execute(new GetUserPictureCmd(userId));
	  }

	  public virtual void deleteUserPicture(string userId)
	  {
		commandExecutor.execute(new DeleteUserPictureCmd(userId));
	  }

	  public virtual string AuthenticatedUserId
	  {
		  set
		  {
			Authentication = new Authentication(value, null);
		  }
	  }

	  public virtual Authentication Authentication
	  {
		  set
		  {
			if (value == null)
			{
			  clearAuthentication();
			}
			else
			{
			  if (!string.ReferenceEquals(value.UserId, null))
			  {
				EnsureUtil.ensureValidIndividualResourceId("Invalid user id provided", value.UserId);
			  }
			  if (value.GroupIds != null)
			  {
				EnsureUtil.ensureValidIndividualResourceIds("At least one invalid group id provided", value.GroupIds);
			  }
			  if (value.TenantIds != null)
			  {
				EnsureUtil.ensureValidIndividualResourceIds("At least one invalid tenant id provided", value.TenantIds);
			  }
    
			  currentAuthentication.set(value);
			}
		  }
	  }

	  public virtual void setAuthentication(string userId, IList<string> groups)
	  {
		Authentication = new Authentication(userId, groups);
	  }

	  public virtual void setAuthentication(string userId, IList<string> groups, IList<string> tenantIds)
	  {
		Authentication = new Authentication(userId, groups, tenantIds);
	  }

	  public virtual void clearAuthentication()
	  {
		currentAuthentication.remove();
	  }

	  public virtual Authentication CurrentAuthentication
	  {
		  get
		  {
			return currentAuthentication.get();
		  }
	  }

	  public virtual string getUserInfo(string userId, string key)
	  {
		return commandExecutor.execute(new GetUserInfoCmd(userId, key));
	  }

	  public virtual IList<string> getUserInfoKeys(string userId)
	  {
		return commandExecutor.execute(new GetUserInfoKeysCmd(userId, IdentityInfoEntity.TYPE_USERINFO));
	  }

	  public virtual IList<string> getUserAccountNames(string userId)
	  {
		return commandExecutor.execute(new GetUserInfoKeysCmd(userId, IdentityInfoEntity.TYPE_USERACCOUNT));
	  }

	  public virtual void setUserInfo(string userId, string key, string value)
	  {
		commandExecutor.execute(new SetUserInfoCmd(userId, key, value));
	  }

	  public virtual void deleteUserInfo(string userId, string key)
	  {
		commandExecutor.execute(new DeleteUserInfoCmd(userId, key));
	  }

	  public virtual void deleteUserAccount(string userId, string accountName)
	  {
		commandExecutor.execute(new DeleteUserInfoCmd(userId, accountName));
	  }

	  public virtual Account getUserAccount(string userId, string userPassword, string accountName)
	  {
		return commandExecutor.execute(new GetUserAccountCmd(userId, userPassword, accountName));
	  }

	  public virtual void setUserAccount(string userId, string userPassword, string accountName, string accountUsername, string accountPassword, IDictionary<string, string> accountDetails)
	  {
		commandExecutor.execute(new SetUserInfoCmd(userId, userPassword, accountName, accountUsername, accountPassword, accountDetails));
	  }

	  public virtual void createTenantUserMembership(string tenantId, string userId)
	  {
		commandExecutor.execute(new CreateTenantUserMembershipCmd(tenantId, userId));
	  }

	  public virtual void createTenantGroupMembership(string tenantId, string groupId)
	  {
		commandExecutor.execute(new CreateTenantGroupMembershipCmd(tenantId, groupId));
	  }

	  public virtual void deleteTenantUserMembership(string tenantId, string userId)
	  {
		commandExecutor.execute(new DeleteTenantUserMembershipCmd(tenantId, userId));
	  }

	  public virtual void deleteTenantGroupMembership(string tenantId, string groupId)
	  {
		commandExecutor.execute(new DeleteTenantGroupMembershipCmd(tenantId, groupId));
	  }

	}

}