using System;
using System.Threading;

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
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using FieldDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.FieldDeclaration;
	using StreamSource = org.camunda.bpm.engine.impl.util.io.StreamSource;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class EngineUtilLogger : ProcessEngineLogger
	{


	  public virtual ProcessEngineException malformedUrlException(string url, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("001", "The URL '{}' is malformed", url), cause);
	  }

	  public virtual ProcessEngineException multipleSourcesException(StreamSource source1, StreamSource source2)
	  {
		return new ProcessEngineException(exceptionMessage("002", "Multiple sources detected, which is invalid. Source 1: '{}', Source 2: {}", source1, source2));
	  }

	  public virtual ProcessEngineException parsingFailureException(string name, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("003", "Could not parse '{}'. {}", name, cause.Message), cause);
	  }

	  public virtual void logParseWarnings(string formattedMessage)
	  {
		logWarn("004", "Warnings during parsing: {}", formattedMessage);
	  }

	  public virtual ProcessEngineException exceptionDuringParsing(string @string)
	  {
		return new ProcessEngineException(exceptionMessage("005", "Could not parse BPMN process. Errors: {}", @string));
	  }


	  public virtual void unableToSetSchemaResource(Exception cause)
	  {
		logWarn("006", "Setting schema resource failed because of: '{}'", cause.Message, cause);
	  }

	  public virtual ProcessEngineException invalidBitNumber(int bitNumber)
	  {
		return new ProcessEngineException(exceptionMessage("007", "Invalid bit {}. Only 8 bits are supported.", bitNumber));
	  }

	  public virtual ProcessEngineException exceptionWhileInstantiatingClass(string className, Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("008", "Exception while instantiating class '{}': {}", className, e.Message), e);
	  }

	  public virtual ProcessEngineException exceptionWhileApplyingFieldDeclatation(string declName, string className, Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("009", "Exception while applying field declaration '{}' on class '{}': {}", declName, className, e.Message), e);
	  }

	  public virtual ProcessEngineException incompatibleTypeForFieldDeclaration(FieldDeclaration declaration, object target, System.Reflection.FieldInfo field)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return new ProcessEngineException(exceptionMessage("010", "Incompatible type set on field declaration '{}' for class '{}'. Declared value has type '{}', while expecting '{}'", declaration.Name, target.GetType().FullName, declaration.Value.GetType().FullName, field.Type.Name));
	  }

	  public virtual ProcessEngineException exceptionWhileReadingStream(string inputStreamName, Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("011", "Exception while reading {} as input stream: {}", inputStreamName, e.Message), e);
	  }

	  public virtual ProcessEngineException exceptionWhileReadingFile(string filePath, Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("012", "Exception while reading file {}: {}", filePath, e.Message), e);
	  }

	  public virtual ProcessEngineException exceptionWhileGettingFile(string filePath, Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("013", "Exception while getting file {}: {}", filePath, e.Message), e);
	  }

	  public virtual ProcessEngineException exceptionWhileWritingToFile(string filePath, Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("014", "Exception while writing to file {}: {}", filePath, e.Message), e);
	  }

	  public virtual void debugCloseException(IOException ignore)
	  {
		logDebug("015", "Ignored exception on resource close", ignore);
	  }

	  public virtual void debugClassLoading(string className, string classLoaderDescription, ClassLoader classLoader)
	  {
		logDebug("016", "Attempting to load class '{}' with {}: {}", className, classLoaderDescription, classLoader);
	  }

	  public virtual ClassLoadingException classLoadingException(string className, Exception throwable)
	  {
		return new ClassLoadingException(exceptionMessage("017", "Cannot load class '{}': {}", className, throwable.Message), className, throwable);
	  }

	  public virtual ProcessEngineException cannotConvertUrlToUri(URL url, URISyntaxException e)
	  {
		return new ProcessEngineException(exceptionMessage("018", "Cannot convert URL[{}] to URI: {}", url, e.Message), e);
	  }

	  public virtual ProcessEngineException exceptionWhileInvokingMethod(string methodName, object target, Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("019", "Exception while invoking method '{}' on object of type '{}': {}'", methodName, target, e.Message), e);
	  }

	  public virtual ProcessEngineException unableToAccessField(System.Reflection.FieldInfo field, string name)
	  {
		return new ProcessEngineException(exceptionMessage("020", "Unable to access field {} on class {}, access protected", field, name));
	  }

	  public virtual ProcessEngineException unableToAccessMethod(string methodName, string name)
	  {
		return new ProcessEngineException(exceptionMessage("021", "Unable to access method {} on class {}, access protected", methodName, name));
	  }

	  public virtual ProcessEngineException exceptionWhileSettingField(System.Reflection.FieldInfo field, object @object, object value, Exception e)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return new ProcessEngineException(exceptionMessage("022", "Exception while setting value '{}' to field '{}' on object of type '{}': {}", value, field, @object.GetType().FullName, e.Message), e);

	  }

	  public virtual ProcessEngineException ambiguousSetterMethod(string setterName, string name)
	  {
		return new ProcessEngineException(exceptionMessage("023", "Ambiguous setter: more than one method named {} on class {}, with different parameter types.", setterName, name));
	  }

	  public virtual NotFoundException cannotFindResource(string resourcePath)
	  {
		return new NotFoundException(exceptionMessage("024", "Unable to find resource at path {}", resourcePath));
	  }

	  public virtual System.InvalidOperationException notInsideCommandContext(string operation)
	  {
		return new System.InvalidOperationException(exceptionMessage("025", "Operation {} requires active command context. No command context active on thread {}.", operation, Thread.CurrentThread));
	  }

	  public virtual ProcessEngineException exceptionWhileParsingCronExpresison(string duedateDescription, Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("026", "Exception while parsing cron expression '{}': {}", duedateDescription, e.Message), e);
	  }

	  public virtual ProcessEngineException exceptionWhileResolvingDuedate(string duedate, Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("027", "Exception while resolving duedate '{}': {}", duedate, e.Message), e);
	  }

	  public virtual Exception cannotParseDuration(string expressions)
	  {
		return new ProcessEngineException(exceptionMessage("028", "Cannot parse duration '{}'.", expressions));
	  }

	  public virtual void logParsingRetryIntervals(string intervals, Exception e)
	  {
		logWarn("029", "Exception while parsing retry intervals '{}'", intervals, e.Message, e);
	  }

	  public virtual void logJsonException(Exception e)
	  {
		logDebug("030", "Exception while parsing JSON: {}", e.Message, e);
	  }
	}

}