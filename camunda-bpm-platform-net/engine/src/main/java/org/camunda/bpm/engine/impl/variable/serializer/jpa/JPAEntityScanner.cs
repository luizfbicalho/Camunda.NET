using System;
using System.Reflection;

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
namespace org.camunda.bpm.engine.impl.variable.serializer.jpa
{



	/// <summary>
	/// Scans class and creates <seealso cref="EntityMetaData"/> based on it. 
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class JPAEntityScanner
	{

	  public virtual EntityMetaData scanClass(Type clazz)
	  {
		EntityMetaData metaData = new EntityMetaData();
		metaData.EntityClass = clazz;

		// Class should have @Entity annotation
		bool isEntity = isEntityAnnotationPresent(clazz);
		metaData.JPAEntity = isEntity;

		if (isEntity)
		{
		  // Try to find a field annotated with @Id
		  System.Reflection.FieldInfo idField = getIdField(clazz);
		  if (idField != null)
		  {
			metaData.IdField = idField;
		  }
		  else
		  {
			// Try to find a method annotated with @Id
			System.Reflection.MethodInfo idMethod = getIdMethod(clazz);
			if (idMethod != null)
			{
			  metaData.IdMethod = idMethod;
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  throw new ProcessEngineException("Cannot find field or method with annotation @Id on class '" + clazz.FullName + "', only single-valued primary keys are supported on JPA-enities");
			}
		  }
		}
		return metaData;
	  }

	  private System.Reflection.MethodInfo getIdMethod(Type clazz)
	  {
		System.Reflection.MethodInfo idMethod = null;
		// Get all public declared methods on the class. According to spec, @Id should only be 
		// applied to fields and property get methods
		System.Reflection.MethodInfo[] methods = clazz.GetMethods();
		Id idAnnotation = null;
		foreach (System.Reflection.MethodInfo method in methods)
		{
		  idAnnotation = method.getAnnotation(typeof(Id));
		  if (idAnnotation != null)
		  {
			idMethod = method;
			break;
		  }
		}
		return idMethod;
	  }

	  private System.Reflection.FieldInfo getIdField(Type clazz)
	  {
	   System.Reflection.FieldInfo idField = null;
	   System.Reflection.FieldInfo[] fields = clazz.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
	   Id idAnnotation = null;
	   foreach (System.Reflection.FieldInfo field in fields)
	   {
		 idAnnotation = field.getAnnotation(typeof(Id));
		 if (idAnnotation != null)
		 {
		   idField = field;
		   break;
		 }
	   }

	   if (idField == null)
	   {
		 // Check superClass for fields with @Id, since getDeclaredFields does
		 // not return superclass-fields.
		 Type superClass = clazz.BaseType;
		 if (superClass != null && !superClass.Equals(typeof(object)))
		 {
		   // Recursively go up class hierarchy
		   idField = getIdField(superClass);
		 }
	   }
	   return idField;
	  }

	  private bool isEntityAnnotationPresent(Type clazz)
	  {
		return (clazz.getAnnotation(typeof(Entity)) != null);
	  }
	}

}