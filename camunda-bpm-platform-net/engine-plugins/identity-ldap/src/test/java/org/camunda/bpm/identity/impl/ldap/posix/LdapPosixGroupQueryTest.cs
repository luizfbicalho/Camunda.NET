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
namespace org.camunda.bpm.identity.impl.ldap.posix
{
	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;


	/// <summary>
	/// @author Tom Crossland
	/// </summary>
	public class LdapPosixGroupQueryTest : LdapPosixTest
	{

	  public virtual void testFilterByGroupId()
	  {
		Group group = identityService.createGroupQuery().groupId("posix-group-without-members").singleResult();
		assertNotNull(group);

		group = identityService.createGroupQuery().groupId("posix-group-with-members").singleResult();
		assertNotNull(group);

		IList<User> result = identityService.createUserQuery().memberOfGroup("posix-group-without-members").list();
		assertEquals(0, result.Count);

		result = identityService.createUserQuery().memberOfGroup("posix-group-with-members").list();
		assertEquals(3, result.Count);
	  }

	}

}