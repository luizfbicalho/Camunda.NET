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
	using ClassFilter = org.springframework.aop.ClassFilter;
	using MethodMatcher = org.springframework.aop.MethodMatcher;
	using Pointcut = org.springframework.aop.Pointcut;
	using AnnotationClassFilter = org.springframework.aop.support.annotation.AnnotationClassFilter;
	using Assert = org.springframework.util.Assert;

	/// <summary>
	/// this code is taken almost (99.99%) verbatim from the Spring Integration
	/// project's source code where it's a static
	/// private inner class.
	/// 
	/// @author Mark Fisher
	/// </summary>
	public class MetaAnnotationMatchingPointcut : Pointcut
	{

		private readonly ClassFilter classFilter;

		private readonly MethodMatcher methodMatcher;


		/// <summary>
		/// Create a new MetaAnnotationMatchingPointcut for the given annotation type.
		/// </summary>
		/// <param name="classAnnotationType"> the annotation type to look for at the class level </param>
		/// <param name="checkInherited">			whether to explicitly check the superclasses and
		///                            interfaces for the annotation type as well (even if the annotation type
		///                            is not marked as inherited itself) </param>
		public MetaAnnotationMatchingPointcut(Type classAnnotationType, bool checkInherited)
		{
			this.classFilter = new AnnotationClassFilter(classAnnotationType, checkInherited);
			this.methodMatcher = MethodMatcher.TRUE;
		}

		/// <summary>
		/// Create a new MetaAnnotationMatchingPointcut for the given annotation type.
		/// </summary>
		/// <param name="classAnnotationType">	the annotation type to look for at the class level
		///                             (can be <code>null</code>) </param>
		/// <param name="methodAnnotationType"> the annotation type to look for at the method level
		///                             (can be <code>null</code>) </param>
		public MetaAnnotationMatchingPointcut(Type classAnnotationType, Type methodAnnotationType)
		{

			Assert.isTrue((classAnnotationType != null || methodAnnotationType != null), "Either Class annotation type or Method annotation type needs to be specified (or both)");

			if (classAnnotationType != null)
			{
				this.classFilter = new AnnotationClassFilter(classAnnotationType);
			}
			else
			{
				this.classFilter = ClassFilter.TRUE;
			}

			if (methodAnnotationType != null)
			{
				this.methodMatcher = new MetaAnnotationMethodMatcher(methodAnnotationType);
			}
			else
			{
				this.methodMatcher = MethodMatcher.TRUE;
			}
		}


		public virtual ClassFilter ClassFilter
		{
			get
			{
				return this.classFilter;
			}
		}

		public virtual MethodMatcher MethodMatcher
		{
			get
			{
				return this.methodMatcher;
			}
		}
	}


}