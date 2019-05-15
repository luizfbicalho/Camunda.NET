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
namespace org.camunda.bpm.engine.impl.util
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using FieldDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.FieldDeclaration;
	using Context = org.camunda.bpm.engine.impl.context.Context;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ClassDelegateUtil
	{

	  private static readonly EngineUtilLogger LOG = ProcessEngineLogger.UTIL_LOGGER;

	  public static object instantiateDelegate(Type clazz, IList<FieldDeclaration> fieldDeclarations)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return instantiateDelegate(clazz.FullName, fieldDeclarations);
	  }

	  public static object instantiateDelegate(string className, IList<FieldDeclaration> fieldDeclarations)
	  {
		ArtifactFactory artifactFactory = Context.ProcessEngineConfiguration.ArtifactFactory;

		try
		{
		  Type clazz = ReflectUtil.loadClass(className);

		  object @object = artifactFactory.getArtifact(clazz);

		  applyFieldDeclaration(fieldDeclarations, @object);
		  return @object;
		}
		catch (Exception e)
		{
		  throw LOG.exceptionWhileInstantiatingClass(className, e);
		}

	  }

	  public static void applyFieldDeclaration(IList<FieldDeclaration> fieldDeclarations, object target)
	  {
		if (fieldDeclarations != null)
		{
		  foreach (FieldDeclaration declaration in fieldDeclarations)
		  {
			applyFieldDeclaration(declaration, target);
		  }
		}
	  }

	  public static void applyFieldDeclaration(FieldDeclaration declaration, object target)
	  {
		System.Reflection.MethodInfo setterMethod = ReflectUtil.getSetter(declaration.Name, target.GetType(), declaration.Value.GetType());

		if (setterMethod != null)
		{
		  try
		  {
			setterMethod.invoke(target, declaration.Value);
		  }
		  catch (Exception e)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw LOG.exceptionWhileApplyingFieldDeclatation(declaration.Name, target.GetType().FullName, e);
		  }
		}
		else
		{
		  System.Reflection.FieldInfo field = ReflectUtil.getField(declaration.Name, target);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  ensureNotNull("Field definition uses unexisting field '" + declaration.Name + "' on class " + target.GetType().FullName, "field", field);
		  // Check if the delegate field's type is correct
		  if (!fieldTypeCompatible(declaration, field))
		  {
			throw LOG.incompatibleTypeForFieldDeclaration(declaration, target, field);
		  }
		  ReflectUtil.setField(field, target, declaration.Value);
		}
	  }

	  public static bool fieldTypeCompatible(FieldDeclaration declaration, System.Reflection.FieldInfo field)
	  {
		if (declaration.Value != null)
		{
		  return declaration.Value.GetType().IsAssignableFrom(field.Type);
		}
		else
		{
		  // Null can be set any field type
		  return true;
		}
	  }

	}

}