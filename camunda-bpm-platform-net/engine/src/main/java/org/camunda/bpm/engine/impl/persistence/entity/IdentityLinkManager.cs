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


	/// <summary>
	/// @author Tom Baeyens
	/// @author Saeid Mirzaei
	/// </summary>
	public class IdentityLinkManager : AbstractManager
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<IdentityLinkEntity> findIdentityLinksByTaskId(String taskId)
	  public virtual IList<IdentityLinkEntity> findIdentityLinksByTaskId(string taskId)
	  {
		return DbEntityManager.selectList("selectIdentityLinksByTask", taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<IdentityLinkEntity> findIdentityLinksByProcessDefinitionId(String processDefinitionId)
	  public virtual IList<IdentityLinkEntity> findIdentityLinksByProcessDefinitionId(string processDefinitionId)
	  {
		return DbEntityManager.selectList("selectIdentityLinksByProcessDefinition", processDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<IdentityLinkEntity> findIdentityLinkByTaskUserGroupAndType(String taskId, String userId, String groupId, String type)
	  public virtual IList<IdentityLinkEntity> findIdentityLinkByTaskUserGroupAndType(string taskId, string userId, string groupId, string type)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["taskId"] = taskId;
		parameters["userId"] = userId;
		parameters["groupId"] = groupId;
		parameters["type"] = type;
		return DbEntityManager.selectList("selectIdentityLinkByTaskUserGroupAndType", parameters);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<IdentityLinkEntity> findIdentityLinkByProcessDefinitionUserAndGroup(String processDefinitionId, String userId, String groupId)
	  public virtual IList<IdentityLinkEntity> findIdentityLinkByProcessDefinitionUserAndGroup(string processDefinitionId, string userId, string groupId)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["processDefinitionId"] = processDefinitionId;
		parameters["userId"] = userId;
		parameters["groupId"] = groupId;
		return DbEntityManager.selectList("selectIdentityLinkByProcessDefinitionUserAndGroup", parameters);
	  }

	  public virtual void deleteIdentityLinksByProcDef(string processDefId)
	  {
		DbEntityManager.delete(typeof(IdentityLinkEntity), "deleteIdentityLinkByProcDef", processDefId);
	  }

	}

}