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
namespace org.camunda.bpm.engine.impl.util
{

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using ProcessElementInstance = org.camunda.bpm.engine.runtime.ProcessElementInstance;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public sealed class StringUtil
	{

	  /// <summary>
	  /// Note: <seealso cref="String.length()"/> counts Unicode supplementary
	  /// characters twice, so for a String consisting only of those,
	  /// the limit is effectively MAX_LONG_STRING_LENGTH / 2
	  /// </summary>
	  public static int DB_MAX_STRING_LENGTH = 666;

	  /// <summary>
	  /// Checks whether a <seealso cref="string"/> seams to be an expression or not
	  /// 
	  /// Note: In most cases you should check for composite expressions. See
	  /// <seealso cref="isCompositeExpression(string, ExpressionManager)"/> for more information.
	  /// </summary>
	  /// <param name="text"> the text to check </param>
	  /// <returns> true if the text seams to be an expression false otherwise </returns>
	  public static bool isExpression(string text)
	  {
		text = text.Trim();
		return text.StartsWith("${", StringComparison.Ordinal) || text.StartsWith("#{", StringComparison.Ordinal);
	  }

	  /// <summary>
	  /// Checks whether a <seealso cref="string"/> seams to be a composite expression or not. In contrast to an eval expression
	  /// is the composite expression also allowed to consist of a combination of literal and eval expressions, e.g.,
	  /// "Welcome ${customer.name} to our site".
	  /// 
	  /// Note: If you just want to allow eval expression, then the expression must always start with "#{" or "${".
	  /// Use <seealso cref="isExpression(string)"/> to conduct these kind of checks.
	  /// 
	  /// </summary>
	  public static bool isCompositeExpression(string text, ExpressionManager expressionManager)
	  {
		return !expressionManager.createExpression(text).LiteralText;
	  }

	  public static string[] split(string text, string regex)
	  {
		if (string.ReferenceEquals(text, null))
		{
		  return null;
		}
		else if (string.ReferenceEquals(regex, null))
		{
		  return new string[] {text};
		}
		else
		{
		  string[] result = text.Split(regex, true);
		  for (int i = 0; i < result.Length; i++)
		  {
			result[i] = result[i].Trim();
		  }
		  return result;
		}
	  }

	  public static bool hasAnySuffix(string text, string[] suffixes)
	  {
		foreach (string suffix in suffixes)
		{
		  if (text.EndsWith(suffix, StringComparison.Ordinal))
		  {
			return true;
		  }
		}

		return false;
	  }

	  /// <summary>
	  /// converts a byte array into a string using the current process engines default charset as
	  /// returned by <seealso cref="ProcessEngineConfigurationImpl.getDefaultCharset()"/>
	  /// </summary>
	  /// <param name="bytes"> the byte array </param>
	  /// <returns> a string representing the bytes </returns>
	  public static string fromBytes(sbyte[] bytes)
	  {
		EnsureUtil.ensureActiveCommandContext("StringUtil.fromBytes");
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		return fromBytes(bytes, processEngineConfiguration.ProcessEngine);
	  }

	  /// <summary>
	  /// converts a byte array into a string using the provided process engine's default charset as
	  /// returned by <seealso cref="ProcessEngineConfigurationImpl.getDefaultCharset()"/>
	  /// </summary>
	  /// <param name="bytes"> the byte array </param>
	  /// <param name="processEngine"> the process engine </param>
	  /// <returns> a string representing the bytes </returns>
	  public static string fromBytes(sbyte[] bytes, ProcessEngine processEngine)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = ((ProcessEngineImpl) processEngine).ProcessEngineConfiguration;
		Charset charset = processEngineConfiguration.DefaultCharset;
		return StringHelper.NewString(bytes, charset);
	  }

	  public static Reader readerFromBytes(sbyte[] bytes)
	  {
		EnsureUtil.ensureActiveCommandContext("StringUtil.readerFromBytes");
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		MemoryStream inputStream = new MemoryStream(bytes);

		return new StreamReader(inputStream, processEngineConfiguration.DefaultCharset);
	  }

	  public static Writer writerForStream(Stream outStream)
	  {
		EnsureUtil.ensureActiveCommandContext("StringUtil.readerFromBytes");
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;

		return new StreamWriter(outStream, processEngineConfiguration.DefaultCharset);
	  }


	  /// <summary>
	  /// Gets the bytes from a string using the current process engine's default charset
	  /// </summary>
	  /// <param name="string"> the string to get the bytes form </param>
	  /// <returns> the byte array </returns>
	  public static sbyte[] toByteArray(string @string)
	  {
		EnsureUtil.ensureActiveCommandContext("StringUtil.toByteArray");
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		return toByteArray(@string, processEngineConfiguration.ProcessEngine);
	  }

	  /// <summary>
	  /// Gets the bytes from a string using the provided process engine's default charset
	  /// </summary>
	  /// <param name="string"> the string to get the bytes form </param>
	  /// <param name="processEngine"> the process engine to use </param>
	  /// <returns> the byte array </returns>
	  public static sbyte[] toByteArray(string @string, ProcessEngine processEngine)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = ((ProcessEngineImpl) processEngine).ProcessEngineConfiguration;
		Charset charset = processEngineConfiguration.DefaultCharset;
		return @string.GetBytes(charset);
	  }

	  /// <summary>
	  /// Trims the input to the <seealso cref="DB_MAX_STRING_LENGTH maxium length allowed"/> for persistence with our default database schema 
	  /// </summary>
	  /// <param name="string"> the input that might be trimmed if maximum length is exceeded </param>
	  /// <returns> the input, eventually trimmed to <seealso cref="DB_MAX_STRING_LENGTH"/> </returns>
	  public static string trimToMaximumLengthAllowed(string @string)
	  {
		if (!string.ReferenceEquals(@string, null) && @string.Length > DB_MAX_STRING_LENGTH)
		{
		  return @string.Substring(0, DB_MAX_STRING_LENGTH);
		}
		return @string;
	  }

	  public static string joinDbEntityIds<T1>(ICollection<T1> dbEntities) where T1 : org.camunda.bpm.engine.impl.db.DbEntity
	  {
		return join(new StringIteratorAnonymousInnerClass(dbEntities.GetEnumerator()));
	  }

	  private class StringIteratorAnonymousInnerClass : StringIterator<DbEntity>
	  {
		  public StringIteratorAnonymousInnerClass(UnknownType iterator) : base(iterator)
		  {
		  }

		  public string next()
		  {
			return iterator.next().Id;
		  }
	  }

	  public static string joinProcessElementInstanceIds<T1>(ICollection<T1> processElementInstances) where T1 : org.camunda.bpm.engine.runtime.ProcessElementInstance
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<? extends org.camunda.bpm.engine.runtime.ProcessElementInstance> iterator = processElementInstances.iterator();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
		IEnumerator<ProcessElementInstance> iterator = processElementInstances.GetEnumerator();
		return join(new StringIteratorAnonymousInnerClass2(iterator));
	  }

	  private class StringIteratorAnonymousInnerClass2 : StringIterator<ProcessElementInstance>
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private System.Collections.Generic.IEnumerator<JavaToDotNetGenericWildcard extends org.camunda.bpm.engine.runtime.ProcessElementInstance> iterator;
		  private new IEnumerator<ProcessElementInstance> iterator;

		  public StringIteratorAnonymousInnerClass2<T1>(IEnumerator<T1> iterator) where T1 : org.camunda.bpm.engine.runtime.ProcessElementInstance : base(iterator)
		  {
			  this.iterator = iterator;
		  }

		  public string next()
		  {
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			return iterator.next().Id;
		  }
	  }

	  public static string join(IEnumerator<string> iterator)
	  {
		StringBuilder builder = new StringBuilder();

		while (iterator.MoveNext())
		{
		  builder.Append(iterator.Current);
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		  if (iterator.hasNext())
		  {
			builder.Append(", ");
		  }
		}

		return builder.ToString();
	  }

	  public abstract class StringIterator<T> : IEnumerator<string>
	  {

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Iterator<? extends T> iterator;
		protected internal IEnumerator<T> iterator;

		public StringIterator<T1>(IEnumerator<T1> iterator) where T1 : T
		{
		  this.iterator = iterator;
		}

		public virtual bool hasNext()
		{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		  return iterator.hasNext();
		}

		public virtual void remove()
		{
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
		  iterator.remove();
		}
	  }

	}

}