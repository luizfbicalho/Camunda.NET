using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

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
namespace org.camunda.bpm.identity.impl.ldap
{
	using DefaultDirectoryService = org.apache.directory.server.core.DefaultDirectoryService;
	using DirectoryService = org.apache.directory.server.core.api.DirectoryService;
	using Entry = org.apache.directory.api.ldap.model.entry.Entry;
	using Partition = org.apache.directory.server.core.api.partition.Partition;
	using JdbmIndex = org.apache.directory.server.core.partition.impl.btree.jdbm.JdbmIndex;
	using JdbmPartition = org.apache.directory.server.core.partition.impl.btree.jdbm.JdbmPartition;
	using LdapServer = org.apache.directory.server.ldap.LdapServer;
	using TcpTransport = org.apache.directory.server.protocol.shared.transport.TcpTransport;
	using Index = org.apache.directory.server.xdbm.Index;
	using Dn = org.apache.directory.api.ldap.model.name.Dn;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	using SchemaManager = org.apache.directory.api.ldap.model.schema.SchemaManager;
	using SchemaLoader = org.apache.directory.api.ldap.model.schema.registries.SchemaLoader;
	using SchemaLdifExtractor = org.apache.directory.api.ldap.schema.extractor.SchemaLdifExtractor;
	using DefaultSchemaLdifExtractor = org.apache.directory.api.ldap.schema.extractor.impl.DefaultSchemaLdifExtractor;
	using LdifSchemaLoader = org.apache.directory.api.ldap.schema.loader.LdifSchemaLoader;
	using DefaultSchemaManager = org.apache.directory.api.ldap.schema.manager.impl.DefaultSchemaManager;
	using Exceptions = org.apache.directory.api.util.exception.Exceptions;
	using ServerDNConstants = org.apache.directory.server.constants.ServerDNConstants;
	using CacheService = org.apache.directory.server.core.api.CacheService;
	using DnFactory = org.apache.directory.server.core.api.DnFactory;
	using InstanceLayout = org.apache.directory.server.core.api.InstanceLayout;
	using SchemaPartition = org.apache.directory.server.core.api.schema.SchemaPartition;
	using LdifPartition = org.apache.directory.server.core.partition.ldif.LdifPartition;
	using I18n = org.apache.directory.server.i18n.I18n;

	using FileUtils = org.apache.commons.io.FileUtils;

	/// <summary>
	/// <para>
	/// LDAP test setup using apache directory</para>
	/// 
	/// @author Bernd Ruecker
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class LdapTestEnvironment
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger LOG = LoggerFactory.getLogger(typeof(LdapTestEnvironment).FullName);

	  private const string BASE_DN = "o=camunda,c=org";

	  protected internal DirectoryService service;
	  protected internal LdapServer ldapService;
	  protected internal string configFilePath = "ldap.properties";
	  protected internal File workingDirectory = new File(System.getProperty("java.io.tmpdir") + "/server-work");

	  public LdapTestEnvironment()
	  {
	  }

	  /// <summary>
	  /// initialize the schema manager and add the schema partition to directory
	  /// service
	  /// </summary>
	  /// <exception cref="Exception"> if the schema LDIF files are not found on the classpath </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void initSchemaPartition() throws Exception
	  protected internal virtual void initSchemaPartition()
	  {
		InstanceLayout instanceLayout = service.InstanceLayout;

		File schemaPartitionDirectory = new File(instanceLayout.PartitionsDirectory, "schema");

		// Extract the schema on disk (a brand new one) and load the registries
		if (schemaPartitionDirectory.exists())
		{
		  LOG.info("schema partition already exists, skipping schema extraction");
		}
		else
		{
		  SchemaLdifExtractor extractor = new DefaultSchemaLdifExtractor(instanceLayout.PartitionsDirectory);
		  extractor.extractOrCopy();
		}

		SchemaLoader loader = new LdifSchemaLoader(schemaPartitionDirectory);
		SchemaManager schemaManager = new DefaultSchemaManager(loader);

		// We have to load the schema now, otherwise we won't be able
		// to initialize the Partitions, as we won't be able to parse
		// and normalize their suffix Dn
		schemaManager.loadAllEnabled();

		IList<Exception> errors = schemaManager.Errors;

		if (errors.Count > 0)
		{
		  throw new Exception(I18n.err(I18n.ERR_317, Exceptions.printErrors(errors)));
		}

		service.SchemaManager = schemaManager;

		// Init the LdifPartition with schema
		LdifPartition schemaLdifPartition = new LdifPartition(schemaManager, service.DnFactory);
		schemaLdifPartition.PartitionPath = schemaPartitionDirectory.toURI();

		// The schema partition
		SchemaPartition schemaPartition = new SchemaPartition(schemaManager);
		schemaPartition.WrappedPartition = schemaLdifPartition;
		service.SchemaPartition = schemaPartition;
	  }

	  /// <summary>
	  /// Initialize the server. It creates the partition, adds the index, and
	  /// injects the context entries for the created partitions.
	  /// </summary>
	  /// <exception cref="Exception"> if there were some problems while initializing the system </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void initializeDirectory() throws Exception
	  protected internal virtual void initializeDirectory()
	  {

		workingDirectory.mkdirs();

		service = new DefaultDirectoryService();
		InstanceLayout il = new InstanceLayout(workingDirectory);
		service.InstanceLayout = il;

		CacheService cacheService = new CacheService();
		cacheService.initialize(service.InstanceLayout);
		service.CacheService = cacheService;

		initSchemaPartition();

		// then the system partition
		// this is a MANDATORY partition
		// DO NOT add this via addPartition() method, trunk code complains about duplicate partition
		// while initializing
		JdbmPartition systemPartition = new JdbmPartition(service.SchemaManager, service.DnFactory);
		systemPartition.Id = "system";
		systemPartition.PartitionPath = (new File(service.InstanceLayout.PartitionsDirectory, systemPartition.Id)).toURI();
		systemPartition.SuffixDn = new Dn(ServerDNConstants.SYSTEM_DN);
		systemPartition.SchemaManager = service.SchemaManager;

		// mandatory to call this method to set the system partition
		// Note: this system partition might be removed from trunk
		service.SystemPartition = systemPartition;

		// Disable the ChangeLog system
		service.ChangeLog.Enabled = false;
		service.DenormalizeOpAttrsEnabled = true;

		Partition camundaPartition = addPartition("camunda", BASE_DN, service.DnFactory);
		addIndex(camundaPartition, "objectClass", "ou", "uid");

		service.startup();

		// Create the root entry
		if (!service.AdminSession.exists(camundaPartition.SuffixDn))
		{
		  Dn dn = new Dn(BASE_DN);
		  Entry entry = service.newEntry(dn);
		  entry.add("objectClass", "top", "domain", "extensibleObject");
		  entry.add("dc", "camunda");
		  service.AdminSession.add(entry);
		}
	  }

	  /// <summary>
	  /// starts the LdapServer
	  /// </summary>
	  /// <exception cref="Exception"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startServer() throws Exception
	  public virtual void startServer()
	  {
		ldapService = new LdapServer();
		Properties properties = loadTestProperties();
		string port = properties.getProperty("ldap.server.port");
		ldapService.Transports = new TcpTransport(int.Parse(port));
		ldapService.DirectoryService = service;
		ldapService.start();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void init() throws Exception
	  public virtual void init()
	  {
		initializeDirectory();
		startServer();

		createGroup("office-berlin");
		string dnRoman = createUserUid("roman", "office-berlin", "Roman", "Smirnov", "roman@camunda.org");
		string dnRobert = createUserUid("robert", "office-berlin", "Robert", "Gimbel", "robert@camunda.org");
		string dnDaniel = createUserUid("daniel", "office-berlin", "Daniel", "Meyer", "daniel@camunda.org");
		string dnGonzo = createUserUid("gonzo", "office-berlin", "Gonzo", "The Great", "gonzo@camunda.org");
		string dnRowlf = createUserUid("rowlf", "office-berlin", "Rowlf", "The Dog", "rowlf@camunda.org");
		string dnPepe = createUserUid("pepe", "office-berlin", "Pepe", "The King Prawn", "pepe@camunda.org");
		string dnRizzo = createUserUid("rizzo", "office-berlin", "Rizzo", "The Rat", "rizzo@camunda.org");

		createGroup("office-london");
		string dnOscar = createUserUid("oscar", "office-london", "Oscar", "The Crouch", "oscar@camunda.org");
		string dnMonster = createUserUid("monster", "office-london", "Cookie", "Monster", "monster@camunda.org");

		createGroup("office-home");
		// Doesn't work using backslashes, end up with two uid attributes
		// See https://issues.apache.org/jira/browse/DIRSERVER-1442
		string dnDavid = createUserUid("david(IT)", "office-home", "David", "Howe\\IT\\", "david@camunda.org");

		string dnRuecker = createUserUid("ruecker", "office-home", "Bernd", "Ruecker", "ruecker@camunda.org");

		createGroup("office-external");
		string dnFozzie = createUserCN("fozzie", "office-external", "Bear", "Fozzie", "fozzie@camunda.org");

		createRole("management", dnRuecker, dnRobert, dnDaniel);
		createRole("development", dnRoman, dnDaniel, dnOscar);
		createRole("consulting", dnRuecker);
		createRole("sales", dnRuecker, dnMonster, dnDavid);
		createRole("external", dnFozzie);
		createRole("all", dnRuecker, dnRobert, dnDaniel, dnRoman, dnOscar, dnMonster, dnDavid, dnFozzie, dnGonzo, dnRowlf, dnPepe, dnRizzo);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected String createUserUid(String user, String group, String firstname, String lastname, String email) throws Exception
	  protected internal virtual string createUserUid(string user, string group, string firstname, string lastname, string email)
	  {
		Dn dn = new Dn("uid=" + user + ",ou=" + group + ",o=camunda,c=org");
		createUser(user, firstname, lastname, email, dn);
		return dn.NormName;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected String createUserCN(String user, String group, String firstname, String lastname, String email) throws Exception
	  protected internal virtual string createUserCN(string user, string group, string firstname, string lastname, string email)
	  {
		Dn dn = new Dn("cn=" + lastname + "\\," + firstname + ",ou=" + group + ",o=camunda,c=org");
		createUser(user, firstname, lastname, email, dn);
		return dn.NormName;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void createUser(String user, String firstname, String lastname, String email, org.apache.directory.api.ldap.model.name.Dn dn) throws Exception, javax.naming.NamingException, UnsupportedEncodingException
	  protected internal virtual void createUser(string user, string firstname, string lastname, string email, Dn dn)
	  {
		if (!service.AdminSession.exists(dn))
		{
		  Entry entry = service.newEntry(dn);
		  entry.add("objectClass", "top", "person", "inetOrgPerson"); //, "extensibleObject"); //make extensible to allow for the "memberOf" field
		  entry.add("uid", user);
		  entry.add("cn", firstname);
		  entry.add("sn", lastname);
		  entry.add("mail", email);
		  entry.add("userPassword", user.GetBytes(Encoding.UTF8));
		  service.AdminSession.add(entry);
		  Console.WriteLine("created entry: " + dn.NormName);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void createGroup(String name) throws javax.naming.InvalidNameException, Exception, javax.naming.NamingException
	  public virtual void createGroup(string name)
	  {
		Dn dn = new Dn("ou=" + name + ",o=camunda,c=org");
		if (!service.AdminSession.exists(dn))
		{
		  Entry entry = service.newEntry(dn);
		  entry.add("objectClass", "top", "organizationalUnit");
		  entry.add("ou", name);
		  service.AdminSession.add(entry);
		  Console.WriteLine("created entry: " + dn.NormName);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void createRole(String roleName, String... users) throws Exception
	  protected internal virtual void createRole(string roleName, params string[] users)
	  {
		Dn dn = new Dn("ou=" + roleName + ",o=camunda,c=org");
		if (!service.AdminSession.exists(dn))
		{
		  Entry entry = service.newEntry(dn);
		  entry.add("objectClass", "top", "groupOfNames");
		  entry.add("cn", roleName);
		  foreach (string user in users)
		  {
			entry.add("member", user);
		  }
		  service.AdminSession.add(entry);
		}
	  }

	  /// <summary>
	  /// Add a new partition to the server
	  /// </summary>
	  /// <param name="partitionId"> The partition Id </param>
	  /// <param name="partitionDn"> The partition DN </param>
	  /// <param name="dnFactory"> the DN factory </param>
	  /// <returns> The newly added partition </returns>
	  /// <exception cref="Exception"> If the partition can't be added </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.apache.directory.server.core.api.partition.Partition addPartition(String partitionId, String partitionDn, org.apache.directory.server.core.api.DnFactory dnFactory) throws Exception
	  protected internal virtual Partition addPartition(string partitionId, string partitionDn, DnFactory dnFactory)
	  {
		// Create a new partition with the given partition id
		JdbmPartition partition = new JdbmPartition(service.SchemaManager, dnFactory);
		partition.Id = partitionId;
		partition.PartitionPath = (new File(service.InstanceLayout.PartitionsDirectory, partitionId)).toURI();
		partition.SuffixDn = new Dn(partitionDn);
		service.addPartition(partition);

		return partition;
	  }

	  /// <summary>
	  /// Add a new set of index on the given attributes
	  /// </summary>
	  /// <param name="partition"> The partition on which we want to add index </param>
	  /// <param name="attrs"> The list of attributes to index </param>
	  protected internal virtual void addIndex(Partition partition, params string[] attrs)
	  {
		// Index some attributes on the apache partition
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Set<org.apache.directory.server.xdbm.Index<?, String>> indexedAttributes = new java.util.HashSet<>();
		ISet<Index<object, string>> indexedAttributes = new HashSet<Index<object, string>>();

		foreach (string attribute in attrs)
		{
		  indexedAttributes.Add(new JdbmIndex<string>(attribute, false));
		}

		((JdbmPartition) partition).IndexedAttributes = indexedAttributes;
	  }

	  public virtual void shutdown()
	  {
		try
		{
		  ldapService.stop();
		  service.shutdown();
		  if (workingDirectory.exists())
		  {
			FileUtils.deleteDirectory(workingDirectory);
		  }
		}
		catch (Exception e)
		{
		  LOG.error("exception while shutting down ldap", e);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.util.Properties loadTestProperties() throws IOException
	  protected internal virtual Properties loadTestProperties()
	  {
		Properties properties = new Properties();
		File file = IoUtil.getFile(configFilePath);
		FileStream propertiesStream = null;
		try
		{
		  propertiesStream = new FileStream(file, FileMode.Open, FileAccess.Read);
		  properties.load(propertiesStream);
		}
		finally
		{
		  IoUtil.closeSilently(propertiesStream);
		}
		return properties;
	  }

	}

}