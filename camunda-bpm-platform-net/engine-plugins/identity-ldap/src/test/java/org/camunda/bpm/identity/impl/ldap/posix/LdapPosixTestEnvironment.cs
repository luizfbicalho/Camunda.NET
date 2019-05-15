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
namespace org.camunda.bpm.identity.impl.ldap.posix
{
	using Entry = org.apache.directory.api.ldap.model.entry.Entry;

	using Modification = org.apache.directory.api.ldap.model.entry.Modification;
	using Attribute = org.apache.directory.api.ldap.model.entry.Attribute;
	using ModificationOperation = org.apache.directory.api.ldap.model.entry.ModificationOperation;
	using Dn = org.apache.directory.api.ldap.model.name.Dn;

	using DefaultModification = org.apache.directory.api.ldap.model.entry.DefaultModification;

	/// <summary>
	/// <para>
	/// LDAP test setup for posix groups using apache directory</para>
	/// 
	/// @author Tom Crossland
	/// </summary>
	public class LdapPosixTestEnvironment : LdapTestEnvironment
	{

	  public LdapPosixTestEnvironment() : base()
	  {
		// overwrite the name of the directory to use
		workingDirectory = new File(System.getProperty("java.io.tmpdir") + "/ldap-posix-work");
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void init() throws Exception
	  public override void init()
	  {
		initializeDirectory();

		// Enable POSIX groups in ApacheDS
		Dn nis = new Dn("cn=nis,ou=schema");
		if (service.AdminSession.exists(nis))
		{
		  Entry entry = service.AdminSession.lookup(nis);
		  Attribute nisDisabled = entry.get("m-disabled");
		  if (null != nisDisabled && "TRUE".Equals(nisDisabled.String, StringComparison.OrdinalIgnoreCase))
		  {
			nisDisabled.remove("TRUE");
			nisDisabled.add("FALSE");
			IList<Modification> modifications = new List<Modification>();
			modifications.Add(new DefaultModification(ModificationOperation.REPLACE_ATTRIBUTE, nisDisabled));
			service.AdminSession.modify(nis, modifications);
			service.shutdown();
			initializeDirectory(); // Note: This instantiates service again for schema modifications to take effect.
		  }
		}

		startServer();

		createGroup("office-berlin");
		createUserUid("daniel", "office-berlin", "Daniel", "Meyer", "daniel@camunda.org");

		createGroup("people");
		createUserUid("ruecker", "people", "Bernd", "Ruecker", "ruecker@camunda.org");
		createUserUid("monster", "people", "Cookie", "Monster", "monster@camunda.org");
		createUserUid("fozzie", "people", "Bear", "Fozzie", "fozzie@camunda.org");

		createGroup("groups");
		createPosixGroup("1", "posix-group-without-members");
		createPosixGroup("2", "posix-group-with-members", "fozzie", "monster", "ruecker");
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void createPosixGroup(String gid, String name, String... memberUids) throws Exception
	  protected internal virtual void createPosixGroup(string gid, string name, params string[] memberUids)
	  {
		Dn dn = new Dn("cn=" + name + ",ou=groups,o=camunda,c=org");
		if (!service.AdminSession.exists(dn))
		{
		  Entry entry = service.newEntry(dn);
		  entry.add("objectClass", "top", "posixGroup");
		  entry.add("cn", name);
		  entry.add("gidNumber", gid);
		  foreach (string memberUid in memberUids)
		  {
			entry.add("memberUid", memberUid);
		  }
		  service.AdminSession.add(entry);
		}
	  }
	}

}