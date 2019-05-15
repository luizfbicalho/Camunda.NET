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
namespace org.camunda.bpm.engine.test.api.identity
{

	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;
	using Assert = org.junit.Assert;

	/// <summary>
	/// @author Simon Jonischkeit
	/// 
	/// </summary>
	public class WriteMultipleEntitiesInOneTransactionTest : ResourceProcessEngineTestCase
	{

	  public WriteMultipleEntitiesInOneTransactionTest() : base("org/camunda/bpm/engine/test/api/identity/WriteMultipleEntitiesInOneTransactionTest.camunda.cfg.xml")
	  {
	  }

	  public virtual void testWriteMultipleEntitiesInOneTransaction()
	  {

		// the identity service provider registered with the engine creates a user, a group, and a membership
		// in the following call:
		Assert.assertTrue(identityService.checkPassword("multipleEntities", "inOneStep"));
		User user = identityService.createUserQuery().userId("multipleEntities").singleResult();

		Assert.assertNotNull(user);
		Assert.assertEquals("multipleEntities", user.Id);
		Assert.assertEquals("{SHA}pfdzmt+49nwknTy7xhZd7ZW5suI=", user.Password);

		// It is expected, that the User is in exactly one Group
		IList<Group> groups = this.identityService.createGroupQuery().groupMember("multipleEntities").list();
		Assert.assertEquals(1, groups.Count);

		Group group = groups[0];
		Assert.assertEquals("multipleEntities_group", group.Id);

		// clean the Db
		identityService.deleteMembership("multipleEntities", "multipleEntities_group");
		identityService.deleteGroup("multipleEntities_group");
		identityService.deleteUser("multipleEntities");
	  }
	}

}