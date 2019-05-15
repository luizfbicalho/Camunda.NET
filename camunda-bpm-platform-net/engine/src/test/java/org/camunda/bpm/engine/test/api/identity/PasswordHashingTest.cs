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
	using User = org.camunda.bpm.engine.identity.User;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using org.camunda.bpm.engine.impl.digest;
	using org.camunda.bpm.engine.test.api.identity.util;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using org.junit;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNot.not;

	public class PasswordHashingTest
	{

	  protected internal static ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal static ProcessEngineTestRule testRule = new ProcessEngineTestRule(engineRule);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain = RuleChain.outerRule(engineRule).around(testRule);

	  protected internal const string PASSWORD = "password";
	  protected internal const string USER_NAME = "johndoe";
	  protected internal const string ALGORITHM_NAME = "awesome";

	  protected internal IdentityService identityService;
	  protected internal RuntimeService runtimeService;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

	  protected internal PasswordEncryptor camundaDefaultEncryptor;
	  protected internal IList<PasswordEncryptor> camundaDefaultPasswordChecker;
	  protected internal SaltGenerator camundaDefaultSaltGenerator;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initialize()
	  public virtual void initialize()
	  {
		runtimeService = engineRule.RuntimeService;
		identityService = engineRule.IdentityService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		camundaDefaultEncryptor = processEngineConfiguration.PasswordEncryptor;
		camundaDefaultPasswordChecker = processEngineConfiguration.CustomPasswordChecker;
		camundaDefaultSaltGenerator = processEngineConfiguration.SaltGenerator;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		removeAllUser();
		resetEngineConfiguration();
	  }

	  protected internal virtual void removeAllUser()
	  {
		IList<User> list = identityService.createUserQuery().list();
		foreach (User user in list)
		{
		  identityService.deleteUser(user.Id);
		}
	  }

	  protected internal virtual void resetEngineConfiguration()
	  {
		setEncryptors(camundaDefaultEncryptor, camundaDefaultPasswordChecker);
		processEngineConfiguration.SaltGenerator = camundaDefaultSaltGenerator;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void saltHashingOnHashedPasswordWithoutSaltThrowsNoError()
	  public virtual void saltHashingOnHashedPasswordWithoutSaltThrowsNoError()
	  {
		// given
		processEngineConfiguration.SaltGenerator = new MyConstantSaltGenerator(null);
		User user = identityService.newUser(USER_NAME);
		user.Password = PASSWORD;

		// when
		identityService.saveUser(user);

		// then
		assertThat(identityService.checkPassword(USER_NAME, PASSWORD), @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void enteringTheSamePasswordShouldProduceTwoDifferentEncryptedPassword()
	  public virtual void enteringTheSamePasswordShouldProduceTwoDifferentEncryptedPassword()
	  {
		// given
		User user1 = identityService.newUser(USER_NAME);
		user1.Password = PASSWORD;
		identityService.saveUser(user1);

		// when
		User user2 = identityService.newUser("kermit");
		user2.Password = PASSWORD;
		identityService.saveUser(user2);

		// then
		assertThat(user1.Password, @is(not(user2.Password)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void ensurePasswordIsCorrectlyHashedWithSHA1()
	  public virtual void ensurePasswordIsCorrectlyHashedWithSHA1()
	  {
		// given
		DefaultEncryptor = new ShaHashDigest();
		processEngineConfiguration.SaltGenerator = new MyConstantSaltGenerator("12345678910");
		User user = identityService.newUser(USER_NAME);
		user.Password = PASSWORD;
		identityService.saveUser(user);

		// when
		user = identityService.createUserQuery().userId(USER_NAME).singleResult();

		// then
		// obtain the expected value on the command line like so: echo -n password12345678910 | openssl dgst -binary -sha1 | openssl base64
		assertThat(user.Password, @is("{SHA}n3fE9/7XOmgD3BkeJlC+JLyb/Qg="));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void ensurePasswordIsCorrectlyHashedWithSHA512()
	  public virtual void ensurePasswordIsCorrectlyHashedWithSHA512()
	  {
		// given
		processEngineConfiguration.SaltGenerator = new MyConstantSaltGenerator("12345678910");
		User user = identityService.newUser(USER_NAME);
		user.Password = PASSWORD;
		identityService.saveUser(user);

		// when
		user = identityService.createUserQuery().userId(USER_NAME).singleResult();

		// then
		// obtain the expected value on the command line like so: echo -n password12345678910 | openssl dgst -binary -sha512 | openssl base64
		assertThat(user.Password, @is("{SHA-512}sM1U4nCzoDbdUugvJ7dJ6rLc7t1ZPPsnAbUpTqi5nXCYp7PTZCHExuzjoxLLYoUK" + "Gd637jKqT8d9tpsZs3K5+g=="));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void twoEncryptorsWithSamePrefixThrowError()
	  public virtual void twoEncryptorsWithSamePrefixThrowError()
	  {

		// given two algorithms with the same prefix
		IList<PasswordEncryptor> additionalEncryptorsForPasswordChecking = new LinkedList<PasswordEncryptor>();
		additionalEncryptorsForPasswordChecking.Add(new ShaHashDigest());
		PasswordEncryptor defaultEncryptor = new ShaHashDigest();

		// then
		thrown.expect(typeof(PasswordEncryptionException));
		thrown.expectMessage("Hash algorithm with the name 'SHA' was already added");

		// when
		setEncryptors(defaultEncryptor, additionalEncryptorsForPasswordChecking);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void prefixThatCannotBeResolvedThrowsError()
	  public virtual void prefixThatCannotBeResolvedThrowsError()
	  {
		// given
		DefaultEncryptor = new MyCustomPasswordEncryptorCreatingPrefixThatCannotBeResolved();
		User user = identityService.newUser(USER_NAME);
		user.Password = PASSWORD;
		identityService.saveUser(user);
		user = identityService.createUserQuery().userId(USER_NAME).singleResult();

		// then
		thrown.expect(typeof(PasswordEncryptionException));
		thrown.expectMessage("Could not resolve hash algorithm name of a hashed password");

		// when
		identityService.checkPassword(user.Id, PASSWORD);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void plugInCustomPasswordEncryptor()
	  public virtual void plugInCustomPasswordEncryptor()
	  {
		// given
		setEncryptors(new MyCustomPasswordEncryptor(PASSWORD, ALGORITHM_NAME), System.Linq.Enumerable.Empty<PasswordEncryptor>());
		User user = identityService.newUser(USER_NAME);
		user.Password = PASSWORD;
		identityService.saveUser(user);

		// when
		user = identityService.createUserQuery().userId(USER_NAME).singleResult();

		// then
		assertThat(user.Password, @is("{" + ALGORITHM_NAME + "}xxx"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void useSeveralCustomEncryptors()
	  public virtual void useSeveralCustomEncryptors()
	  {

		// given three users with different hashed passwords
		processEngineConfiguration.SaltGenerator = new MyConstantSaltGenerator("12345678910");

		string userName1 = "Kermit";
		createUserWithEncryptor(userName1, new MyCustomPasswordEncryptor(PASSWORD, ALGORITHM_NAME));

		string userName2 = "Fozzie";
		string anotherAlgorithmName = "marvelousAlgorithm";
		createUserWithEncryptor(userName2, new MyCustomPasswordEncryptor(PASSWORD, anotherAlgorithmName));

		string userName3 = "Gonzo";
		createUserWithEncryptor(userName3, new ShaHashDigest());

		IList<PasswordEncryptor> additionalEncryptorsForPasswordChecking = new LinkedList<PasswordEncryptor>();
		additionalEncryptorsForPasswordChecking.Add(new MyCustomPasswordEncryptor(PASSWORD, ALGORITHM_NAME));
		additionalEncryptorsForPasswordChecking.Add(new MyCustomPasswordEncryptor(PASSWORD, anotherAlgorithmName));
		PasswordEncryptor defaultEncryptor = new ShaHashDigest();
		setEncryptors(defaultEncryptor, additionalEncryptorsForPasswordChecking);

		// when
		User user1 = identityService.createUserQuery().userId(userName1).singleResult();
		User user2 = identityService.createUserQuery().userId(userName2).singleResult();
		User user3 = identityService.createUserQuery().userId(userName3).singleResult();

		// then
		assertThat(user1.Password, @is("{" + ALGORITHM_NAME + "}xxx"));
		assertThat(user2.Password, @is("{" + anotherAlgorithmName + "}xxx"));
		assertThat(user3.Password, @is("{SHA}n3fE9/7XOmgD3BkeJlC+JLyb/Qg="));
	  }

	  protected internal virtual void createUserWithEncryptor(string userName, PasswordEncryptor encryptor)
	  {
		setEncryptors(encryptor, System.Linq.Enumerable.Empty<PasswordEncryptor>());
		User user = identityService.newUser(userName);
		user.Password = PASSWORD;
		identityService.saveUser(user);
	  }

	  protected internal virtual PasswordEncryptor DefaultEncryptor
	  {
		  set
		  {
			setEncryptors(value, System.Linq.Enumerable.Empty<PasswordEncryptor>());
		  }
	  }

	  protected internal virtual void setEncryptors(PasswordEncryptor defaultEncryptor, IList<PasswordEncryptor> additionalEncryptorsForPasswordChecking)
	  {
		processEngineConfiguration.PasswordManager = new PasswordManager(defaultEncryptor, additionalEncryptorsForPasswordChecking);
	  }

	}

}