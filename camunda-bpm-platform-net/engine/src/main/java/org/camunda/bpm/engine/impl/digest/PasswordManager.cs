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
namespace org.camunda.bpm.engine.impl.digest
{


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// Different Camunda versions use different hashing algorithms. In addition, it is possible
	/// to add a custom hashing algorithm. The <seealso cref="PasswordManager"/> ensures that the right
	/// algorithm is used for the encryption.
	/// 
	/// Default algorithms:
	/// Version:           |    Algorithm
	/// <= Camunda 7.6     | SHA1
	/// >= Camunda 7.7     | SHA512
	/// </summary>
	public class PasswordManager
	{

	  public static readonly SecurityLogger LOG = ProcessEngineLogger.SECURITY_LOGGER;

	  protected internal IDictionary<string, PasswordEncryptor> passwordChecker = new Dictionary<string, PasswordEncryptor>();
	  protected internal PasswordEncryptor defaultPasswordEncryptor;

	  protected internal DatabasePrefixHandler prefixHandler = new DatabasePrefixHandler();

	  public PasswordManager(PasswordEncryptor defaultPasswordEncryptor, IList<PasswordEncryptor> customPasswordChecker)
	  {
		// add default password encryptors for password checking
		// for Camunda 7.6 and earlier
		addPasswordCheckerAndThrowErrorIfAlreadyAvailable(new ShaHashDigest());
		// from Camunda 7.7
		addPasswordCheckerAndThrowErrorIfAlreadyAvailable(new Sha512HashDigest());

		// add custom encryptors
		addAllPasswordChecker(customPasswordChecker);

		addDefaultEncryptor(defaultPasswordEncryptor);
	  }

	  protected internal virtual void addAllPasswordChecker(IList<PasswordEncryptor> list)
	  {
		foreach (PasswordEncryptor encryptor in list)
		{
		  addPasswordCheckerAndThrowErrorIfAlreadyAvailable(encryptor);
		}
	  }

	  protected internal virtual void addPasswordCheckerAndThrowErrorIfAlreadyAvailable(PasswordEncryptor encryptor)
	  {
		if (passwordChecker.ContainsKey(encryptor.hashAlgorithmName()))
		{
		  throw LOG.hashAlgorithmForPasswordEncryptionAlreadyAvailableException(encryptor.hashAlgorithmName());
		}
		passwordChecker[encryptor.hashAlgorithmName()] = encryptor;
	  }

	  protected internal virtual void addDefaultEncryptor(PasswordEncryptor defaultPasswordEncryptor)
	  {
		this.defaultPasswordEncryptor = defaultPasswordEncryptor;
		passwordChecker[defaultPasswordEncryptor.hashAlgorithmName()] = defaultPasswordEncryptor;
	  }

	  public virtual string encrypt(string password)
	  {
		string prefix = prefixHandler.generatePrefix(defaultPasswordEncryptor.hashAlgorithmName());
		return prefix + defaultPasswordEncryptor.encrypt(password);
	  }

	  public virtual bool check(string password, string encrypted)
	  {
		PasswordEncryptor encryptor = getCorrectEncryptorForPassword(encrypted);
		string encryptedPasswordWithoutPrefix = prefixHandler.removePrefix(encrypted);
		ensureNotNull("encryptedPasswordWithoutPrefix", encryptedPasswordWithoutPrefix);
		return encryptor.check(password, encryptedPasswordWithoutPrefix);
	  }

	  protected internal virtual PasswordEncryptor getCorrectEncryptorForPassword(string encryptedPassword)
	  {
		string hashAlgorithmName = prefixHandler.retrieveAlgorithmName(encryptedPassword);
		if (string.ReferenceEquals(hashAlgorithmName, null) || !passwordChecker.ContainsKey(hashAlgorithmName))
		{
		  throw LOG.cannotResolveAlgorithmPrefixFromGivenPasswordException(hashAlgorithmName, passwordChecker.Keys);
		}
		return passwordChecker[hashAlgorithmName];
	  }

	}

}