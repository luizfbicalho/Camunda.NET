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
namespace org.camunda.bpm.engine.spring.components.aop
{

	using MethodInterceptor = org.aopalliance.intercept.MethodInterceptor;
	using MethodInvocation = org.aopalliance.intercept.MethodInvocation;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using BusinessKey = org.camunda.bpm.engine.spring.annotations.BusinessKey;
	using ProcessVariable = org.camunda.bpm.engine.spring.annotations.ProcessVariable;
	using StartProcess = org.camunda.bpm.engine.spring.annotations.StartProcess;
	using AnnotationUtils = org.springframework.core.annotation.AnnotationUtils;
	using AsyncResult = org.springframework.scheduling.annotation.AsyncResult;
	using Assert = org.springframework.util.Assert;
	using StringUtils = org.springframework.util.StringUtils;

	/// <summary>
	/// <seealso cref="org.aopalliance.intercept.MethodInterceptor"/> that starts a business process
	/// as a result of a successful method invocation.
	/// 
	/// @author Josh Long
	/// </summary>
	public class ProcessStartingMethodInterceptor : MethodInterceptor
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		private Logger log = Logger.getLogger(this.GetType().FullName);

		/// <summary>
		/// injected reference - can be obtained via a <seealso cref="org.camunda.bpm.engine.spring.ProcessEngineFactoryBean"/>
		/// </summary>
		protected internal ProcessEngine processEngine;

		/// <param name="processEngine"> takes a reference to a <seealso cref="org.camunda.bpm.engine.ProcessEngine"/> </param>
		public ProcessStartingMethodInterceptor(ProcessEngine processEngine)
		{
			this.processEngine = processEngine;
		}

		internal virtual bool shouldReturnProcessInstance(StartProcess startProcess, MethodInvocation methodInvocation, object result)
		{
			return (result is ProcessInstance || typeof(ProcessInstance).IsAssignableFrom(methodInvocation.Method.ReturnType));
		}

		internal virtual bool shouldReturnProcessInstanceId(StartProcess startProcess, MethodInvocation methodInvocation, object result)
		{
			return startProcess.returnProcessInstanceId() && (result is string || typeof(string).IsAssignableFrom(methodInvocation.Method.ReturnType));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") boolean shouldReturnAsyncResultWithProcessInstance(org.camunda.bpm.engine.spring.annotations.StartProcess startProcess, org.aopalliance.intercept.MethodInvocation methodInvocation, Object result)
		internal virtual bool shouldReturnAsyncResultWithProcessInstance(StartProcess startProcess, MethodInvocation methodInvocation, object result)
		{
			return (result is Future || typeof(Future).IsAssignableFrom(methodInvocation.Method.ReturnType));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object invoke(org.aopalliance.intercept.MethodInvocation invocation) throws Throwable
		public virtual object invoke(MethodInvocation invocation)
		{

			System.Reflection.MethodInfo method = invocation.Method;

			StartProcess startProcess = AnnotationUtils.getAnnotation(method, typeof(StartProcess));

			string processKey = startProcess.processKey();

			Assert.hasText(processKey, "you must provide the name of process to start");

			object result;
			try
			{
				result = invocation.proceed();
				IDictionary<string, object> vars = this.processVariablesFromAnnotations(invocation);

				string businessKey = this.processBusinessKey(invocation);

				log.info("variables for the started process: " + vars.ToString());

				RuntimeService runtimeService = this.processEngine.RuntimeService;
				ProcessInstance pi;
				if (null != businessKey && StringUtils.hasText(businessKey))
				{
					pi = runtimeService.startProcessInstanceByKey(processKey, businessKey, vars);
					log.info("the business key for the started process is '" + businessKey + "' ");
				}
				else
				{
					pi = runtimeService.startProcessInstanceByKey(processKey, vars);
				}

				string pId = pi.Id;

				if (invocation.Method.ReturnType.Equals(typeof(void)))
				{
					return null;
				}

				if (shouldReturnProcessInstance(startProcess, invocation, result))
				{
					return pi;
				}

				if (shouldReturnProcessInstanceId(startProcess, invocation, result))
				{
					return pId;
				}

				if (shouldReturnAsyncResultWithProcessInstance(startProcess, invocation, result))
				{
					return new AsyncResult<ProcessInstance>(pi);
				}

			}
			catch (Exception th)
			{
				throw new Exception(th);
			}
			return result;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected String processBusinessKey(org.aopalliance.intercept.MethodInvocation invocation) throws Throwable
		protected internal virtual string processBusinessKey(MethodInvocation invocation)
		{
			IDictionary<BusinessKey, string> businessKeyAnnotations = this.mapOfAnnotationValues(typeof(BusinessKey),invocation);
			if (businessKeyAnnotations.Count == 1)
			{
				BusinessKey processId = businessKeyAnnotations.Keys.GetEnumerator().next();
				return businessKeyAnnotations[processId];
			}
			return null;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private <K extends Annotation, V> java.util.Map<K, V> mapOfAnnotationValues(Class<K> annotationType, org.aopalliance.intercept.MethodInvocation invocation)
		private IDictionary<K, V> mapOfAnnotationValues<K, V>(Type annotationType, MethodInvocation invocation) where K : Annotation
		{
				annotationType = typeof(K);
			System.Reflection.MethodInfo method = invocation.Method;
			Annotation[][] annotations = method.ParameterAnnotations;
			IDictionary<K, V> vars = new Dictionary<K, V>();
			int paramIndx = 0;
			foreach (Annotation[] annPerParam in annotations)
			{
				foreach (Annotation annotation in annPerParam)
				{
					if (!annotationType.IsAssignableFrom(annotation.GetType()))
					{
						continue;
					}
					K pv = (K) annotation;
					V v = (V) invocation.Arguments[paramIndx];
					vars[pv] = v;

				}
				paramIndx += 1;
			}
			return vars;
		}


		/// <summary>
		/// if there any arguments with the <seealso cref="org.camunda.bpm.engine.annotations.ProcessVariable"/> annotation,
		/// then we feed those parameters into the business process
		/// </summary>
		/// <param name="invocation"> the invocation of the method as passed to the <seealso cref="org.aopalliance.intercept.MethodInterceptor.invoke(org.aopalliance.intercept.MethodInvocation)"/> method </param>
		/// <returns> returns the map of process variables extracted from the parameters </returns>
		/// <exception cref="Throwable"> thrown anything goes wrong </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.util.Map<String, Object> processVariablesFromAnnotations(org.aopalliance.intercept.MethodInvocation invocation) throws Throwable
		protected internal virtual IDictionary<string, object> processVariablesFromAnnotations(MethodInvocation invocation)
		{

			IDictionary<ProcessVariable, object> vars = this.mapOfAnnotationValues(typeof(ProcessVariable), invocation);

			IDictionary<string, object> varNameToValueMap = new Dictionary<string, object>();
			foreach (ProcessVariable processVariable in vars.Keys)
			{
				varNameToValueMap[processVariable.value()] = vars[processVariable];
			}
			return varNameToValueMap;

		}
	}

}