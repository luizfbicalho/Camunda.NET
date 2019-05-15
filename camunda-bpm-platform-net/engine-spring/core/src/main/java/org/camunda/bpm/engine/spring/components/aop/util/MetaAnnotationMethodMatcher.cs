using System;

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
namespace org.camunda.bpm.engine.spring.components.aop.util
{
	using AopUtils = org.springframework.aop.support.AopUtils;
	using AnnotationMethodMatcher = org.springframework.aop.support.annotation.AnnotationMethodMatcher;
	using AnnotationUtils = org.springframework.core.annotation.AnnotationUtils;


	/// <summary>
	/// this code is taken almost verbatim from the Spring Integration
	/// project's source code where it's a static
	/// private inner class.
	/// 
	/// @author Mark Fisher
	/// 
	/// </summary>
	public class MetaAnnotationMethodMatcher : AnnotationMethodMatcher
	{

		private readonly Type annotationType;


		/// <summary>
		/// Create a new AnnotationClassFilter for the given annotation type.
		/// </summary>
		/// <param name="annotationType"> the annotation type to look for </param>
		public MetaAnnotationMethodMatcher(Type annotationType) : base(annotationType)
		{
			this.annotationType = annotationType;
		}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("rawtypes") public boolean matches(Method method, Class targetClass)
		public override bool matches(System.Reflection.MethodInfo method, Type targetClass)
		{
			if (AnnotationUtils.getAnnotation(method, this.annotationType) != null)
			{
				return true;
			}
			// The method may be on an interface, so let's check on the target class as well.
			System.Reflection.MethodInfo specificMethod = AopUtils.getMostSpecificMethod(method, targetClass);
			return (specificMethod != method && (AnnotationUtils.getAnnotation(specificMethod, this.annotationType) != null));
		}
	}

}